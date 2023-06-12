using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Movies.Api;
using Movies.Models;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Movies.Services
{
    public interface IGenresService : IDisposable
    {
        IObservable<bool> ObserveBusy { get; }
        IObservable<Exception> ObserveErrors { get; }
        IObservable<List<Genre>> ObserveGenres { get; }
    }
    
    public class GenresService : IGenresService
    {
        private readonly IApi _api;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly BusyNotifier _busyNotifier = new BusyNotifier();
        private readonly Subject<Exception> _errors;
        private readonly BehaviorSubject<List<Genre>> _genresBehaviorSubject;
        private bool _isInitialized;

        public GenresService(IApi api)
        {
            _api = api;
            _errors = new Subject<Exception>().AddTo(_disposables);
            _genresBehaviorSubject = new BehaviorSubject<List<Genre>>(new List<Genre>()).AddTo(_disposables);
        }

        public IObservable<bool> ObserveBusy => _busyNotifier;
        public IObservable<Exception> ObserveErrors => _errors;
        public IObservable<List<Genre>> ObserveGenres => EnsureInitialized(_genresBehaviorSubject);

        private IObservable<T> EnsureInitialized<T>(IObservable<T> observable)
        {
            if (_isInitialized)
                return observable;

            _isInitialized = true;

            var busy = _busyNotifier.ProcessStart();

            _api.GetGenres()
                .ToObservable()
                .Finally(busy.Dispose)
                .Subscribe(result =>
                {
                    result.Genres.Insert(0, new Genre{Id = -1, Name = "Popular"});
                    _genresBehaviorSubject.OnNext(result.Genres);
                    
                }, ex =>
                {
                    _isInitialized = false;
                    _errors.OnNext(ex);
                })
                .AddTo(_disposables);
            
            return observable;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}