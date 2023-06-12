using System;
using System.Collections.Generic;
using System.Linq;
using Movies.Helpers;
using Movies.Models;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace Movies.Views.Home.Components
{
    public class MovieBindableItem : IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly Movie _movie;
        private readonly Action<MovieBindableItem> _onSelectCommandAction;
        private readonly BusyNotifier _busyNavigation = new BusyNotifier();

        public MovieBindableItem(Movie movie,
                                 bool isPopular,
                                 Action<MovieBindableItem> onSelectCommandAction)
        {
            IsPopular = isPopular;
            _movie = movie;
            _onSelectCommandAction = onSelectCommandAction;

            SelectCommand = _busyNavigation
                .Inverse()
                .ToReactiveCommand()
                .WithSubscribe(OnSelectCommand)
                .AddTo(_disposables);
        }

        public bool IsPopular { get; }
        public int Id => _movie.Id;
        public string Title => _movie.Title;
        public DateTime ReleaseDate => _movie.ReleaseDate;
        public int Ratings => _movie.VoteCount;
        public double VoteAverage => _movie.VoteAverage / 2;
        public string Overview => _movie.Overview;
        public string PosterPath => $"{Secrets.ImageBaseUrl}{_movie.PosterPath}";
        public List<int> GenresIds => _movie.GenreIds;
        
        public ReactiveCommand SelectCommand { get; }

        private void OnSelectCommand()
        {
            var busy = _busyNavigation.ProcessStart();
            _onSelectCommandAction.Invoke(this);
            busy.Dispose();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
    
    public class MovieBindableItemComparer : IEqualityComparer<MovieBindableItem>
    {
        public bool Equals(MovieBindableItem x, MovieBindableItem y)
        {
            return y != null && x != null && x.Id == y.Id;
        }

        public int GetHashCode(MovieBindableItem obj)
        {
            return obj.Id;
        }
    }
}