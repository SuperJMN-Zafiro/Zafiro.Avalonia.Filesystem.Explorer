using System.Linq;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Aggregation;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class SelectionContext : ISelectionContext
{
    private readonly CompositeDisposable disposable = new();

    public SelectionContext(ISelectionHandler<IEntry, string> selectionHandler, IObservable<IZafiroDirectory> directories, IClipboard clipboard, ITransferManager transferManager, INotificationService notificationService)
    {
        selectionHandler.Changes.Bind(out var selectedEntries)
            .Subscribe()
            .DisposeWith(disposable);

        Copy = ReactiveCommand.Create(() =>
        {
            var clipboardItems = selectedEntries.Select(ToClipboardItem).ToList();
            clipboard.Add(clipboardItems);
            return clipboardItems;
        }, selectionHandler.Changes.IsNotEmpty());

        var pasteViewModel = new PasteViewModel(clipboard, directories, transferManager);
        Paste = pasteViewModel.Paste;

        var delete = new DeleteViewModel(selectionHandler.Changes, transferManager);
        Delete = delete.Delete;

        IsPasting = Paste.IsExecuting;
    }

    private static IClipboardItem ToClipboardItem(IEntry entry)
    {
        return entry switch
        {
            DirectoryItemViewModel di => new ClipboardDirectoryItemViewModel(di.Directory),
            FileItemViewModel fi => new ClipboardFileItemViewModel(fi.File),
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };
    }

    public IObservable<bool> IsPasting { get; }

    public ReactiveCommand<Unit, IList<IAction<LongProgress>>> Delete { get; }

    public ReactiveCommand<Unit, IAction<LongProgress>> Paste { get; }

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; }

    [Reactive]
    public bool IsTouchFriendlySelectionEnabled { get; set; }
}