using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.Misc;
using Zafiro.Reactive;
using Zafiro.UI;
using DynamicData.Aggregation;
using Optional;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public class SelectionContext : ReactiveObject, ISelectionHandler
{
    private readonly CompositeDisposable disposables = new();
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit,Unit>> selectAll;
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit,Unit>> selectNone;

    public SelectionContext(IObservable<DirectoryContentsViewModel> directories)
    {
        var selectionChanges = directories
            .Select(x => new ObservableSelectionModel<IDirectoryItem, string>(x.Selection, item => item.Key))
            .DisposePrevious()
            .Select(x => x.Selection)
            .Switch();

        SelectionCount = selectionChanges.Count().Replay().RefCount();
        TotalCount = directories.Select(x => x.Entries).Switch().Count().Replay().RefCount();
        
        SelectionChanges = selectionChanges;
        selectAll = directories.Select(model => ReactiveCommand.Create(() => model.Selection.SelectAll())).DisposePrevious().ToProperty(this, x => x.SelectAll);
        selectNone = directories.Select(model => ReactiveCommand.Create(() => model.Selection.Clear())).DisposePrevious().ToProperty(this, x => x.SelectNone);

        // Copy = CreateCopyCommand(selectionHandler, explorerContext.Clipboard, selectedEntries);
        // Paste = CreatePasteCommand(directories, explorerContext.Clipboard, explorerContext.TransferManager);
        // Delete = CreateDeleteCommand(selectionHandler, explorerContext.TransferManager);
        // IsPasting = Paste.IsExecuting;
    }

    public IObservable<bool> IsPasting { get; }

    // public ReactiveCommand<Unit, IList<ITransferItem>> Delete { get; }
    //
    // public ReactiveCommand<Unit, IList<ITransferItem>> Paste { get; }
    //
    // public ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; }

    [Reactive] public bool IsTouchFriendlySelectionEnabled { get; set; }

    // private static IClipboardItem ToClipboardItem(IEntry entry)
    // {
    //     return entry switch
    //     {
    //         DirectoryItemViewModel di => new ClipboardDirectoryItemViewModel(di.Directory),
    //         FileItemViewModel fi => new ClipboardFileItemViewModel(fi.File),
    //         _ => throw new ArgumentOutOfRangeException(nameof(entry))
    //     };
    // }

    // private ReactiveCommand<Unit, List<IClipboardItem>> CreateCopyCommand(ISelectionHandler<IEntry, string> selectionHandler, IClipboard clipboard, ReadOnlyObservableCollection<IEntry> selectedEntries)
    // {
    //     var command = ReactiveCommand.Create(() => selectedEntries.Select(ToClipboardItem).ToList(), selectionHandler.SelectionChanges.IsNotEmpty());
    //     command.Do(clipboard.Add).Subscribe().DisposeWith(disposables);
    //     return command;
    // }
    //
    // private ReactiveCommand<Unit, IList<ITransferItem>> CreatePasteCommand(IObservable<IZafiroDirectory> directories, IClipboard clipboard, ITransferManager transferManager)
    // {
    //     var pasteViewModel = new PasteViewModel(clipboard, directories, transferManager);
    //     return pasteViewModel.Paste;
    // }
    //
    // private ReactiveCommand<Unit, IList<ITransferItem>> CreateDeleteCommand(ISelectionHandler<IEntry, string> selectionHandler, ITransferManager transferManager)
    // {
    //     var deleteViewModel = new DeleteViewModel(selectionHandler.SelectionChanges, transferManager);
    //     return deleteViewModel.Delete;
    // }
    public ReactiveCommand<Unit, Unit> SelectNone => selectNone.Value;
    public ReactiveCommand<Unit, Unit> SelectAll => selectAll.Value;
    public IObservable<int> SelectionCount { get; }
    public IObservable<int> TotalCount { get; }
    public IObservable<IChangeSet<IDirectoryItem, string>> SelectionChanges { get; }
}