using System;
using System.Reactive.Linq;

namespace Zafiro.Avalonia.FileExplorer.Model;

public static class ObservableExtensions
{
    public static IObservable<T> DelayItem<T>(this IObservable<T> sequence, T itemToDelay, TimeSpan timeSpan) where T : notnull
    {
        return sequence
            .Select(x => x.Equals(itemToDelay) ? Observable.Return(x).Delay(timeSpan) : Observable.Return(x))
            .Switch();
    }
}