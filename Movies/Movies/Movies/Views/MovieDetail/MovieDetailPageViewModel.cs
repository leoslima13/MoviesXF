using System;
using System.Reactive.Threading.Tasks;
using Movies.Views.Home.Components;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Movies.Extensions;

namespace Movies.Views.MovieDetail
{
    public class MovieDetailPageViewModel : BaseViewModel
    {
        public MovieDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            PosterPath = new ReactiveProperty<string>().AddTo(Disposables);
            Title = new ReactiveProperty<string>().AddTo(Disposables);
            ReleasedDate = new ReactiveProperty<DateTime>().AddTo(Disposables);
            Rating = new ReactiveProperty<int>().AddTo(Disposables);
            Overview = new ReactiveProperty<string>().AddTo(Disposables);
            VoteAverage = new ReactiveProperty<double>().AddTo(Disposables);

            BackCommand = new ReactiveCommand()
                .WithSubscribe(() => NavigationService.GoBackAsync().ToObservable().Subscribe().AddTo(Disposables))
                .AddTo(Disposables);
        }
        
        public ReactiveProperty<string> PosterPath { get; }
        public ReactiveProperty<string> Title { get; }
        public ReactiveProperty<DateTime> ReleasedDate { get; }
        public ReactiveProperty<int> Rating { get; }
        public ReactiveProperty<double> VoteAverage { get; }
        public ReactiveProperty<string> Overview { get; }
        
        public ReactiveCommand BackCommand { get; }

        public override void Initialize(INavigationParameters parameters)
        {
            var param = parameters.GetValue<MovieDetailPageParameter>();
            if (param == null)
                throw new Exception(
                    "You must pass MovieDetailPageParameter through the NavigationParameters to use this page");

            var bindableItem = param.MovieBindableItem;

            PosterPath.Value = bindableItem.PosterPath;
            Title.Value = bindableItem.Title;
            ReleasedDate.Value = bindableItem.ReleaseDate;
            Rating.Value = bindableItem.Ratings;
            Overview.Value = bindableItem.Overview;
            VoteAverage.Value = bindableItem.VoteAverage;
        }
    }
    
    public class MovieDetailPageParameter 
    {
        public MovieDetailPageParameter(MovieBindableItem movieBindableItem)
        {
            MovieBindableItem = movieBindableItem;
        }
        
        public MovieBindableItem MovieBindableItem { get; }
    }
}