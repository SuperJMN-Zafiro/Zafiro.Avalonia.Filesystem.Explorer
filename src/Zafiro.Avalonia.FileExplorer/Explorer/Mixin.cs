using System.Linq.Expressions;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Binding;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public static class Mixin
{
    public static IDisposable UpdateCollectionWhenSomeOtherCollectionObservableChanges<T, TItem>(
        this T parent, 
        Expression<Func<T, ReadOnlyObservableCollection<TItem>>> selector, out ReadOnlyObservableCollection<TItem> collection) where TItem : notnull where T : ReactiveObject
    {
        CompositeDisposable disposable = new();
        var source = new SourceList<TItem>()
            .DisposeWith(disposable);

        parent.WhenAnyValue(selector)
            .Do(r => source.EditDiff(r))
            .Select(r => r.ToObservableChangeSet())
            .Switch()
            .PopulateInto(source)
            .DisposeWith(disposable);

        source.Connect()
            .Bind(out collection)
            .Subscribe()
            .DisposeWith(disposable);

        return disposable;
    }
}