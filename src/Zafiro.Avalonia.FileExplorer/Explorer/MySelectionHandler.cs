using Avalonia.Controls.Selection;
using DynamicData;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class MySelectionHandler<T, TKey> : ReactiveObject, ISelectionHandler<T, TKey> where TKey : notnull where T : notnull
{
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit, Unit>> selectAll;
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit, Unit>> selectNone;

    public MySelectionHandler(IObservable<SelectionModel<T>> selectionModels, Func<T, TKey> keySelector)
    {
        var selectionTracker = selectionModels.Select(x => new SelectionTracker<T, TKey>(x, keySelector).Changes).Switch();
        Changes = selectionTracker;
        selectAll = selectionModels.Select(x => ReactiveCommand.Create(x.SelectAll)).ToProperty(this, handler => handler.SelectAll);
        selectNone = selectionModels.Select(x => ReactiveCommand.Create(x.Clear)).ToProperty(this, handler => handler.SelectNone);
    }

    public IObservable<IChangeSet<T, TKey>> Changes { get; }
    public ReactiveCommand<Unit, Unit> SelectNone => selectNone.Value;
    public ReactiveCommand<Unit, Unit> SelectAll => selectAll.Value;
}