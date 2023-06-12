using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Movies.Constants;
using Movies.Controls;
using Movies.Extensions;
using Movies.Models;
using Movies.Services;
using Movies.Views.Home.Components;
using Movies.Views.MovieDetail;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Xamarin.Essentials;

namespace Movies.Views.Home
{
    public class HomePageViewModel : BaseViewModel
    {
        private readonly IMoviesService _moviesService;
        private readonly INetworkMonitorService _networkMonitorService;
        private readonly ILoggerService _loggerService;

        public HomePageViewModel(INavigationService navigationService,
            IMoviesService moviesService,
            INetworkMonitorService networkMonitorService,
            ILoggerService loggerService) : base(navigationService, networkMonitorService)
        {
            _moviesService = moviesService;
            _networkMonitorService = networkMonitorService;
            _loggerService = loggerService;

            AllMovies = new EnhancedReactiveCollection<MoviesGroupCollection>().AddTo(Disposables);

            IsBusy = moviesService
                .ObserveBusy
                .ToReadOnlyReactiveProperty()
                .AddTo(Disposables);

            moviesService
                .ObserveErrors
                .WhereNotNull()
                .Subscribe(OnError)
                .AddTo(Disposables);

            moviesService
                .ObserveMoviesPager
                .WhereNotNull()
                .Where(x => x.Any())
                .Subscribe(OnMoviesPager)
                .AddTo(Disposables);

            RefreshCommand = new ReactiveCommand()
                .WithSubscribe(OnRefreshCommand)
                .AddTo(Disposables);

            LoadMoreCommand = new ReactiveCommand<MoviesGroupCollection>()
                .WithSubscribe(OnLoadMoreCommand)
                .AddTo(Disposables);
        }

        public ReadOnlyReactiveProperty<bool> IsBusy { get; }

        public EnhancedReactiveCollection<MoviesGroupCollection> AllMovies { get; }

        public ReactiveCommand RefreshCommand { get; }
        public ReactiveCommand<MoviesGroupCollection> LoadMoreCommand { get; }

        public override void Initialize(INavigationParameters parameters)
        {
            _moviesService.Initialize();

            IsOffline
                .Skip(1)
                .Where(x => !x)
                .Subscribe(_ =>
                {
                    if(AllMovies.Count == 0)
                        _moviesService.Initialize();
                }).AddTo(Disposables);
        }

        private void OnMoviesPager(List<MoviePager> result)
        {
            var allGroupedMovies = new List<MoviesGroupCollection>();
            var observable = result.ToObservable();

            observable
                .SelectMany(x => x.ObserveMoviesByGenre.WhereNotNull())
                .Where(x => x.Any())
                .Select(x =>
                {
                    var collection = new MoviesGroupCollection(x.Genre, CreateMovieBindableItem(x));
                    allGroupedMovies.Add(collection);
                    return collection;
                })
                .TakeLast(1)
                .Subscribe(_ =>
                {
                    var popular = allGroupedMovies.FirstOrDefault(x => x.Header.IsPopular);

                    if (popular != null)
                    {
                        allGroupedMovies.Remove(popular);
                        allGroupedMovies = allGroupedMovies.OrderBy(x => x.Header.Name).ToList();
                        allGroupedMovies.Insert(0, popular);
                    }

                    AllMovies.ClearOnScheduler();
                    AllMovies.AddRangeOnScheduler(allGroupedMovies);
                }).AddTo(Disposables);

            observable
                .SelectMany(x => x.ObserveNewPageGrouped)
                .Where(x => x != null)
                .Subscribe(x => { OnNewPage(CreateMovieBindableItem(x), x.Genre.Name); })
                .AddTo(Disposables);
        }

        private void OnError(Exception ex)
        {
            _loggerService.Error(ex);
            ShowError("Something went wrong loading movies, try again later!");
        }

        private void OnNewPage(IEnumerable<MovieBindableItem> items, string header)
        {
            var groupOnCollection = AllMovies.FirstOrDefault(x => x.Header.Name == header);
            groupOnCollection?.AddRange(items);
        }

        private List<MovieBindableItem> CreateMovieBindableItem(MoviesByGenre group)
        {
            return group.Select(m =>
                new MovieBindableItem(m, group.Genre.IsPopular, OnMovieSelected)
                    .AddTo(Disposables)).ToList();
        }

        private void OnMovieSelected(MovieBindableItem movie)
        {
            NavigationService.NavigateAsync(Pages.MovieDetailPage,
                    new MovieDetailPageParameter(movie).ToNavigationParameter())
                .ToObservable()
                .Subscribe()
                .AddTo(Disposables);
        }

        private void OnLoadMoreCommand(MoviesGroupCollection collection)
        {
            _moviesService.Page(collection.Header);
        }

        private void OnRefreshCommand()
        {
            if (IsBusy.Value)
                return;

            _moviesService.Initialize();
        }
    }
}