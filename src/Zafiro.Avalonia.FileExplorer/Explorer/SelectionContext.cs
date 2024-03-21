using System.Linq;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Aggregation;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.FileExplorer.TransferManager.Items;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class SelectionContext : ISelectionContext
{
    public ISelectionHandler SelectionHandler { get; }
    private readonly CompositeDisposable disposables = new();

    public SelectionContext(ISelectionHandler<IEntry, string> selectionHandler, IObservable<IZafiroDirectory> directories, ExplorerContext explorerContext)
    {
        SelectionHandler = selectionHandler;
        selectionHandler.Changes
            .Bind(out var selectedEntries)
            .Subscribe()
            .DisposeWith(disposables);
        
        Copy = CreateCopyCommand(selectionHandler, explorerContext.Clipboard, selectedEntries);
        Paste = CreatePasteCommand(directories, explorerContext.Clipboard, explorerContext.TransferManager);
        Delete = CreateDeleteCommand(selectionHandler, explorerContext.TransferManager);
        IsPasting = Paste.IsExecuting;
    }

    public IObservable<bool> IsPasting { get; }

    public ReactiveCommand<Unit, IList<ITransferItem>> Delete { get; }

    public ReactiveCommand<Unit, IList<ITransferItem>> Paste { get; }

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; }

    [Reactive] public bool IsTouchFriendlySelectionEnabled { get; set; }

    private static IClipboardItem ToClipboardItem(IEntry entry)
    {
        return entry switch
        {
            DirectoryItemViewModel di => new ClipboardDirectoryItemViewModel(di.Directory),
            FileItemViewModel fi => new ClipboardFileItemViewModel(fi.File),
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };
    }

    private ReactiveCommand<Unit, List<IClipboardItem>> CreateCopyCommand(ISelectionHandler<IEntry, string> selectionHandler, IClipboard clipboard, ReadOnlyObservableCollection<IEntry> selectedEntries)
    {
        var command = ReactiveCommand.Create(() => selectedEntries.Select(ToClipboardItem).ToList(), selectionHandler.Changes.IsNotEmpty());
        command.Do(clipboard.Add).Subscribe().DisposeWith(disposables);
        return command;
    }

    private ReactiveCommand<Unit, IList<ITransferItem>> CreatePasteCommand(IObservable<IZafiroDirectory> directories, IClipboard clipboard, ITransferManager transferManager)
    {
        var pasteViewModel = new PasteViewModel(clipboard, directories, transferManager);
        return pasteViewModel.Paste;
    }

    private ReactiveCommand<Unit, IList<ITransferItem>> CreateDeleteCommand(ISelectionHandler<IEntry, string> selectionHandler, ITransferManager transferManager)
    {
        var deleteViewModel = new DeleteViewModel(selectionHandler.Changes, transferManager);
        return deleteViewModel.Delete;
    }
}