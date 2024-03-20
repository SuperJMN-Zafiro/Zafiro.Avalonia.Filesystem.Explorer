using System.Linq;
using System.Reactive.Disposables;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer.Address;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class FileSystemExplorer : ReactiveObject, IFileSystemExplorer, IDisposable, ISelectionContext
{
    private readonly ObservableAsPropertyHelper<DirectoryContentsViewModel> details;
    private readonly CompositeDisposable disposable = new();
    private readonly ISelectionContext selectionContext;

    public FileSystemExplorer(IFileSystemRoot fileSystem, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager, IContentOpener opener)
    {
        Clipboard = clipboard;
        PathNavigator = new PathNavigatorViewModel(fileSystem, notificationService);
        TransferManager = transferManager;
        
        var detailsViewModels = PathNavigator.LoadRequestedPath.Successes()
            .Select(directory => new DirectoryContentsViewModel(directory, new EverythingEntryFactory(PathNavigator, opener, notificationService, this), PathNavigator, notificationService, opener, this))
            .ReplayLastActive();

        details = detailsViewModels.ToProperty(this, explorer => explorer.Details)
            .DisposeWith(disposable);

        SerialDisposable serialDisposable = new();
        this.WhenAnyValue(x => x.Details)
            .Do(d => serialDisposable.Disposable = d)
            .Subscribe()
            .DisposeWith(disposable);

        var selectionHandler = new SelectionHandler<IEntry, string>(this.WhenAnyValue(x => x.Details.Selection), x => x.Path);
        var selectContext = new SelectionContext(selectionHandler, PathNavigator.LoadRequestedPath.Successes(), clipboard, transferManager, notificationService);

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

    public ReactiveCommand<Unit, IList<IAction<LongProgress>>> Delete => selectionContext.Delete;

    public ReactiveCommand<Unit, IAction<LongProgress>> Paste => selectionContext.Paste;

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy => selectionContext.Copy;

    [Reactive]
    public bool IsTouchFriendlySelectionEnabled { get; set; }
}