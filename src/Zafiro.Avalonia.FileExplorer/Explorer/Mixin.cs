namespace Zafiro.Avalonia.FileExplorer.Explorer;

public static class Mixin
{
    public static ObservableAsPropertyHelper<T> DisposeMany<T>(this ObservableAsPropertyHelper<T> f, Action<IDisposable> func)
    {
        var observable = f.WhenAnyValue(x => x.Value)
            .WhereNotNull()
            .Select(arg => (object?)arg);
        
        var dis = observable
            .WhereNotNull()
            .OfType<IDisposable>()
            .Skip(1)
            .Do(disposable => disposable.Dispose())
            .Subscribe();

        func(dis);

        return f;
    }
    
    public static IObservable<T> DisposePrevious<T>(this IObservable<T> source) where T : IDisposable
    {
        return Observable.Create<T>(observer =>
        {
            T previous = default;

            return source.Subscribe(
                onNext: item =>
                {
                    if (previous != null)
                    {
                        previous.Dispose();
                    }
                    previous = item;
                    observer.OnNext(item);
                },
                onError: observer.OnError,
                onCompleted: () =>
                {
                    if (previous != null)
                    {
                        previous.Dispose();
                    }
                    observer.OnCompleted();
                });
        });
    }
}