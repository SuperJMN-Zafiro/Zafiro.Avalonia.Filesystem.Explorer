using System.Reactive.Disposables;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public static class ReactiveExtensions
{
    public static IObservable<T> DisposePrevious<T>(this IObservable<T> source) where T : IDisposable
    {
        return Observable.Create<T>(observer =>
        {
            var serialDisposable = new SerialDisposable();
            var subscription = source.Subscribe(
                onNext: value =>
                {
                    serialDisposable.Disposable = value;
                    observer.OnNext(value);
                },
                onError: observer.OnError,
                onCompleted: observer.OnCompleted
            );

            return new CompositeDisposable(subscription, serialDisposable);
        });
    }
}