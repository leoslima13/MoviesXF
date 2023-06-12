using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings.Disposables;

namespace Movies.Views
{
    public abstract class BaseViewModel : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected CompositeDisposable Disposables = new CompositeDisposable();

        protected BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }
        
        protected INavigationService NavigationService { get; }
        
        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        public void Destroy()
        {
            Disposables.Dispose();
        }
    }
}