using System;
using System.Reactive.Linq;
using Movies.Extensions;
using Movies.Services;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using Xamarin.Essentials;

namespace Movies.Views
{
    public abstract class BaseViewModel : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();
        private readonly CompositeDisposable _timerDisposable;

        protected BaseViewModel(INavigationService navigationService,
                                INetworkMonitorService networkMonitorService)
        {
            _timerDisposable = new CompositeDisposable().AddTo(Disposables);
            
            NavigationService = navigationService;
            
            IsOffline = networkMonitorService
                .ObserveNetworkAccess
                .Select(x => x != NetworkAccess.Internet && x != NetworkAccess.ConstrainedInternet)
                .ToReadOnlyReactiveProperty()
                .AddTo(Disposables);

            ErrorMessage = new ReactiveProperty<string>()
                .AddTo(Disposables);

            HasError = ErrorMessage
                .WhereNotNull()
                .Select(x => !string.IsNullOrEmpty(x))
                .ToReadOnlyReactiveProperty()
                .AddTo(Disposables);
        }
        
        protected INavigationService NavigationService { get; }
        
        public ReadOnlyReactiveProperty<bool> IsOffline { get; }
        public ReactiveProperty<string> ErrorMessage { get; }
        public ReadOnlyReactiveProperty<bool> HasError { get; }

        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        protected void ShowError(string message, TimeSpan? timeoutToDismiss = null)
        {
            _timerDisposable.Clear();

            var timeout = timeoutToDismiss ?? TimeSpan.FromSeconds(5);
            ErrorMessage.Value = message;
            Observable.Timer(timeout)
                .Take(1)
                .Subscribe(_ =>
                {
                    ErrorMessage.Value = string.Empty;
                }).AddTo(_timerDisposable);
        }

        public void Destroy()
        {
            Disposables.Dispose();
        }
    }
}