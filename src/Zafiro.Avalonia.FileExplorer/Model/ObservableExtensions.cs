using System.Reactive.Concurrency;

namespace Zafiro.Avalonia.FileExplorer.Model;

public static class ObservableExtensions
{
    public static IObservable<T> DelayItem<T>(this IObservable<T> sequence, T itemToDelay, TimeSpan timeSpan, IScheduler? scheduler = null) where T : notnull
    {
        return sequence
            .Select(x => x.Equals(itemToDelay) ? Observable.Return(x).Delay(timeSpan, scheduler ?? Scheduler.Default) : Observable.Return(x))
            .Switch();
    }
}