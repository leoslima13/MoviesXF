using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Movies.Api;
using Movies.Extensions;
using Movies.Helpers;
using Movies.Models;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Movies.Services
{
    public interface IMoviesService : IDisposable
    {
        IObservable<bool> ObserveBusy { get; }
        IObservable<Exception> ObserveErrors { get; }
        IObservable<List<MoviePager>> ObserveMoviesPager { get; }
        void Initialize();
        void Page(Genre genre);
    }

    public class MoviesService : IMoviesService
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly IGenresService _genresService;
        private readonly IApi _api;
        private readonly INetworkMonitorService _networkMonitorService;
        private readonly BehaviorSubject<List<MoviePager>> _moviesBehaviorSubject;
        private readonly Subject<Exception> _errors;
        private readonly BusyNotifier _busyNotifier = new BusyNotifier();

        public MoviesService(IGenresService genresService, IApi api, INetworkMonitorService networkMonitorService)
        {
            _genresService = genresService;
            _api = api;
            _networkMonitorService = networkMonitorService;

            _moviesBehaviorSubject = new BehaviorSubject<List<MoviePager>>(new List<MoviePager>()).AddTo(_disposables);
            _errors = new Subject<Exception>().AddTo(_disposables);
        }

        public IObservable<bool> ObserveBusy => _busyNotifier.CombineLatestOr(_moviesBehaviorSubject.WhereNotNull()
            .SelectMany(x => x.Select(y => y.ObserveLoading).Merge()));

        public IObservable<Exception> ObserveErrors => _errors
            .Merge(_moviesBehaviorSubject.WhereNotNull()
                .SelectMany(x => x.Select(y => y.ObserveErrors).Merge()))
            .Merge(_genresService.ObserveErrors);

        public IObservable<List<MoviePager>> ObserveMoviesPager => _moviesBehaviorSubject;

        public void Initialize()
        {
            if (!_networkMonitorService.HasInternetConnection)
                return;
            
            var busy = _busyNotifier.ProcessStart();
            var pagers = new List<MoviePager>();
            
            _genresService
                .ObserveGenres
                .Where(x => x.Any())
                .SelectMany(genres =>
                {
                    return genres
                        .ToObservable()
                        .Select(y =>
                        {
                            var moviePager = new MoviePager(_api, y);
                            moviePager.Initialize();

                            pagers.Add(moviePager);
                            return Unit.Default;
                        })
                        .TakeLast(1);
                })
                .Subscribe(_ =>
                {
                    busy.Dispose();
                    _moviesBehaviorSubject.OnNext(pagers);
                }, ex =>
                {
                    busy.Dispose();
                    _errors.OnNext(ex);
                });
        }

        public void Page(Genre genre)
        {
            if (!_networkMonitorService.HasInternetConnection)
                return;
            
            var pager = _moviesBehaviorSubject.Value.FirstOrDefault(m => m.Genre.Id == genre.Id);
            pager?.Page();
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
    
    public class MoviePager : PagerHelper<Movie>
    {
        private readonly IApi _api;

        public MoviePager(IApi api, Genre genre)
        {
            _api = api;
            Genre = genre;
        }

        public Genre Genre { get; }

        public IObservable<MoviesByGenre> ObserveMoviesByGenre =>
            ObserveItems.Select(x => new MoviesByGenre(Genre, x.ToList()));

        public IObservable<MoviesByGenre> ObserveNewPageGrouped =>
            ObserveNewPage.Select(x => new MoviesByGenre(Genre, x.ToList()));

        protected override IObservable<PageResult<Movie>> LoadItems(int nextPage)
        {
            if (Genre.Id == -1)
                return _api.GetPopularMovies(nextPage)
                    .ToObservable();
            
            return _api.GetMoviesWithGenre(nextPage, Genre.Id)
                .ToObservable();
        }
    }
}