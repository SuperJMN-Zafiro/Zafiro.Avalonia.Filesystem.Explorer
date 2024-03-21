using System.Reactive.Disposables;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer.Address;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.FileExplorer.TransferManager.Items;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class FileSystemExplorer : ReactiveObject, IFileSystemExplorer, IDisposable, ISelectionContext
{
    private readonly ObservableAsPropertyHelper<DirectoryContentsViewModel> details;
    private readonly CompositeDisposable disposable = new();
    private readonly ISelectionContext selectionContext;

    public FileSystemExplorer(ExplorerContext explorerContext, IFileSystemRoot fileSystem)
    {
        Clipboard = explorerContext.Clipboard;
        PathNavigator = new PathNavigatorViewModel(fileSystem, explorerContext.NotificationService);
        TransferManager = explorerContext.TransferManager;

        details = PathNavigator.LoadRequestedPath.Successes()
            .Select(directory => new DirectoryContentsViewModel(explorerContext, directory, new EverythingEntryFactory(explorerContext, PathNavigator, this), PathNavigator, this))
            .ToProperty(this, explorer => explorer.Details);

        var selectionHandler = new SelectionHandler<IEntry, string>(this.WhenAnyValue(x => x.Details.Selection), x => x.Path);
        var selectContext = new SelectionContext(selectionHandler, PathNavigator.LoadRequestedPath.Successes(), explorerContext);

        selectionContext = selectContext;
        ToolBar = new ToolBarViewModel(this);

        InitialPath.Or(ZafiroPath.Empty).Execute(GoTo);
    }

    public Maybe<ZafiroPath> InitialPath { get; init; }

    public void Dispose()
    {
        details.Dispose();
    }

    public IObservable<Maybe<IZafiroDirectory>> CurrentDirectory => PathNavigator.CurrentDirectory;

    public ITransferManager TransferManager { get; }

    public IToolBar ToolBar { get; }

    public IPathNavigator PathNavigator { get; }

    public DirectoryContentsViewModel Details => details.Value;

    public IClipboard Clipboard { get; }

    public void GoTo(ZafiroPath path)
    {
        PathNavigator.SetAndLoad(path);
    }

    public IObservable<bool> IsPasting => selectionContext.IsPasting;

    public ReactiveCommand<Unit, IList<ITransferItem>> Delete => selectionContext.Delete;

    public ReactiveCommand<Unit, IList<ITransferItem>> Paste => selectionContext.Paste;

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy => selectionContext.Copy;

    [Reactive] public bool IsTouchFriendlySelectionEnabled { get; set; }
}