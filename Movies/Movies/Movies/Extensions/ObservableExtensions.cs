using System;
using System.Reactive.Linq;

namespace Movies.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<T> WhereNotNull<T>(this IObservable<T> observable)
        {
            return observable.Where(x => x != null);
        }

        public static IObservable<bool> CombineLatestOr(this IObservable<bool> observable, IObservable<bool> other)
        {
            return observable.CombineLatest(other, (a, b) => a || b);
        }
    }
}