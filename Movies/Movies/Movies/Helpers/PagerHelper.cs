using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Movies.Models;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Movies.Helpers
{
    public interface IPagerHelper<T>
    {
        IObservable<bool> ObserveCanLoadMore { get; }
        IObservable<bool> ObserveLoading { get; }
        IObservable<bool> ObserveLoadingPage { get; }
        IObservable<Exception> ObserveErrors { get; }
        IObservable<IEnumerable<T>> ObserveItems { get; }
        IObservable<IEnumerable<T>> ObserveNewPage { get; }
        void Page();
        void Initialize();
    }
    
    public abstract class PagerHelper<T> : IPagerHelper<T>, IDisposable
    {
        protected readonly CompositeDisposable Disposables = new CompositeDisposable();
        private readonly Subject<IEnumerable<T>> _itemsSubject;
        private readonly Subject<IEnumerable<T>> _pageSubject;
        private readonly Subject<Exception> _errors;
        private readonly Subject<bool> _canLoadMore;
        private BusyNotifier _busyLoading = new BusyNotifier();
        private BusyNotifier _busyLoadingMore = new BusyNotifier();
        private int _totalPages, _currentPage;

        protected PagerHelper()
        {
            _itemsSubject = new Subject<IEnumerable<T>>().AddTo(Disposables);
            _pageSubject = new Subject<IEnumerable<T>>().AddTo(Disposables);
            _errors = new Subject<Exception>().AddTo(Disposables);
            _canLoadMore = new Subject<bool>().AddTo(Disposables);
        }

        public IObservable<bool> ObserveCanLoadMore => _canLoadMore;
        public IObservable<bool> ObserveLoading => _busyLoading;
        public IObservable<bool> ObserveLoadingPage => _busyLoadingMore;
        public IObservable<Exception> ObserveErrors => _errors;
        public IObservable<IEnumerable<T>> ObserveItems => _itemsSubject;
        public IObservable<IEnumerable<T>> ObserveNewPage => _pageSubject;

        protected abstract IObservable<PageResult<T>> LoadItems(int nextPage);

        public void Page()
        {
            if (_busyLoadingMore.IsBusy)
                return;
            
            var busy = _busyLoadingMore.ProcessStart();

            LoadItems(++_currentPage)
                .Subscribe(OnPageLoaded, ex =>
                {
                    busy.Dispose();
                    _errors.OnNext(ex);
                })
                .AddTo(Disposables);

            void OnPageLoaded(PageResult<T> pageResult)
            {
                _totalPages = pageResult.TotalPages;
                _canLoadMore.OnNext(_currentPage < _totalPages);
                _pageSubject.OnNext(pageResult.Results);
                busy.Dispose();
            }
        }

        public void Initialize()
        {
            if (_busyLoading.IsBusy)
                return;
            
            _currentPage = 1;
            
            var busy = _busyLoading.ProcessStart();

            LoadItems(_currentPage)
                .Subscribe(OnItemsLoaded, ex =>
                {
                    busy.Dispose();
                    _errors.OnNext(ex);
                })
                .AddTo(Disposables);

            void OnItemsLoaded(PageResult<T> pageResult)
            {
                _totalPages = pageResult.TotalPages;
                _canLoadMore.OnNext(_currentPage < _totalPages);
                _itemsSubject.OnNext(pageResult.Results);
                _itemsSubject.OnCompleted();
                busy.Dispose();
            }
        }

        public void Dispose()
        {
            Disposables.Dispose();
        }
    }
}