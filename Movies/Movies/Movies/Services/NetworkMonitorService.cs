using System;
using System.Reactive;
using System.Reactive.Linq;
using Xamarin.Essentials;

namespace Movies.Services
{
    public interface INetworkMonitorService
    {
        bool HasInternetConnection { get; }
        IObservable<NetworkAccess> ObserveNetworkAccess { get; }
    } 
    
    public class NetworkMonitorService : INetworkMonitorService
    {
        public bool HasInternetConnection => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public IObservable<NetworkAccess> ObserveNetworkAccess => GetFromEventPattern()
            .Select(x => x.EventArgs.NetworkAccess)
            .StartWith(Connectivity.NetworkAccess);
        
        private static IObservable<EventPattern<ConnectivityChangedEventArgs>> GetFromEventPattern()
        {
            return Observable.FromEventPattern<ConnectivityChangedEventArgs>(
                h => Connectivity.ConnectivityChanged += h,
                h => Connectivity.ConnectivityChanged -= h);
        }
    }
}