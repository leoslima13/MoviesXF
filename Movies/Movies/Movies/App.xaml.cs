using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection.Extension;
using FFImageLoading;
using FFImageLoading.Config;
using Microsoft.Extensions.DependencyInjection;
using Movies.Api;
using Movies.Constants;
using Movies.Extensions;
using Movies.Helpers;
using Movies.Services;
using Movies.Views.Home;
using Movies.Views.MovieDetail;
using Polly;
using Polly.Timeout;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Refit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Movies
{
    public partial class App
    {
        public App(IPlatformInitializer initializer) : base(initializer)
        {
            Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }

        protected override void OnInitialized()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            ImageService.Instance.Initialize(new Configuration
            {
                HttpClient = httpClient
            });
            LogUnobservedTaskExceptions();
            
            InitializeComponent();
            Sharpnado.MaterialFrame.Initializer.Initialize(false, false);

            var navPage = new NavigationPage
            {
                BarBackgroundColor = Colors.PrimaryColor,
                BarTextColor = Colors.WhiteColor
            };
            MainPage = navPage;

            NavigationService.NavigateAsync(Pages.HomePage)
                .ToObservable()
                .Subscribe(result =>
                {
                    if (result.Success)
                    {
                        
                    }
                });
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterPopupDialogService();
            
            //Configuring refit, polly, httpclient
            RegisterHttpClientHandlers(containerRegistry.GetContainer());
            
            containerRegistry.RegisterSingleton<IMoviesService, MoviesService>();
            containerRegistry.RegisterSingleton<ILoggerService, LoggerService>();
            containerRegistry.RegisterSingleton<IGenresService, GenresService>();
            containerRegistry.RegisterSingleton<INetworkMonitorService, NetworkMonitorService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<MovieDetailPage, MovieDetailPageViewModel>();
        }
        
        private void LogUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                var loggerService = Container.Resolve<ILoggerService>();
                loggerService.Error(e.Exception.Message, e.Exception);
            };
        }
        
        private void RegisterHttpClientHandlers(IContainer container)
        {
            var refitSettings = new RefitSettings(new NewtonsoftJsonContentSerializer());
            
            var retryPolicy = Policy<HttpResponseMessage>.Handle<HttpRequestException>()
                .OrResult(r => r.StatusCode == HttpStatusCode.RequestTimeout)
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(AppConstants.DefaultRetryCount, i => TimeSpan.FromSeconds(i.ToFibonacci()));

            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();
            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(AppConstants.DefaultTimeoutSeconds);

            container.RegisterServices(services =>
            {
                services.AddSingleton<INetworkMonitorService, NetworkMonitorService>();

                services.AddHttpClient(AppInfo.Name, client =>
                    {
                        client.BaseAddress = new Uri(Secrets.BaseUrl);
                        client.Timeout = TimeSpan.FromSeconds(AppConstants.DefaultTimeoutSeconds);
                        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Secrets.ApiKey}");
                    })
                    .AddTypedClient(c => RestService.For<IApi>(c, refitSettings))
                    .AddPolicyHandler((sp, _) => ((INetworkMonitorService) sp.GetService(typeof(INetworkMonitorService))).HasInternetConnection ? retryPolicy : noOpPolicy)
                    .AddPolicyHandler((sp, _) => ((INetworkMonitorService) sp.GetService(typeof(INetworkMonitorService))).HasInternetConnection ? timeoutPolicy : noOpPolicy);
            });
        }
    }
}

