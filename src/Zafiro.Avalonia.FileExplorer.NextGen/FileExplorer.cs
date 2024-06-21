using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Clipboard;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;
using Zafiro.FileSystem.Mutable;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen;

public class FileExplorer : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<DirectoryContentsViewModel> contents;

    public FileExplorer(IMutableFileSystem mutableFileSystem, INotificationService notificationService, IDialog dialog,
        IClipboardService clipboardService, ITransferManager transferManager)
    {
        MutableFileSystem = mutableFileSystem;
        TransferManager = transferManager;
        PathNavigator = new PathNavigatorViewModel(mutableFileSystem, notificationService);

        var context = new ExplorerContext(PathNavigator, notificationService, mutableFileSystem, dialog, clipboardService);

        ToolBar = new ToolBarViewModel(context);
        contents = context.Directory.ToProperty(this, x => x.Contents);
        PathNavigator.SetAndLoad(mutableFileSystem.InitialPath);
    }

    public ToolBarViewModel ToolBar { get; }

    public DirectoryContentsViewModel Contents => contents.Value;
    public IMutableFileSystem MutableFileSystem { get; }
    public ITransferManager TransferManager { get; }
    public IPathNavigator PathNavigator { get; }
}