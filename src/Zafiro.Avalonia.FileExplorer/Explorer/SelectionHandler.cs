using Avalonia.Controls.Selection;
using DynamicData;
using DynamicData.Aggregation;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class SelectionHandler<T, TKey> : ReactiveObject, ISelectionHandler<T, TKey> where TKey : notnull where T : notnull
{
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit, Unit>> selectAll;
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit, Unit>> selectNone;

    public SelectionHandler(IObservable<SelectionModel<T>> selectionModels, Func<T, TKey> keySelector)
    {
        var trackers = selectionModels.Select(x => new SelectionTracker<T, TKey>(x, keySelector));

        TotalCount = trackers.Select(x => x.Count).Switch();
        Changes = trackers.Select(x => x.Changes).Switch();
        
        selectAll = selectionModels.Select(x => ReactiveCommand.Create(x.SelectAll)).ToProperty(this, handler => handler.SelectAll);
        selectNone = selectionModels.Select(x => ReactiveCommand.Create(x.Clear)).ToProperty(this, handler => handler.SelectNone);


        var selectedCount = Changes.Count();
        selectedCount.Subscribe(i => { });
        SelectionKind = selectedCount.WithLatestFrom(TotalCount, (selected, total) => selected == 0 ? UI.SelectionKind.None : total == selected ? UI.SelectionKind.Full : UI.SelectionKind.Partial);
    }

    public IObservable<int> TotalCount { get; }
    public IObservable<IChangeSet<T, TKey>> Changes { get; }
    public ReactiveCommand<Unit, Unit> SelectNone => selectNone.Value;
    public ReactiveCommand<Unit, Unit> SelectAll => selectAll.Value;
    public IObservable<SelectionKind> SelectionKind { get; }
}
