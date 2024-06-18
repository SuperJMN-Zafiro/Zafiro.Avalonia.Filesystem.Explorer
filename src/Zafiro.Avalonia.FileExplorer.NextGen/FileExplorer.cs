using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer.NextGen.Core;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;
using Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.DynamicData;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen;

public class FileExplorer : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<DirectoryContentsViewModel> contents;

    public FileExplorer(IFileSystem fileSystem, INotificationService notificationService, IDialog dialog)
    {
        FileSystem = fileSystem;
        PathNavigator = new PathNavigatorViewModel(fileSystem, notificationService);
        
        var context = new ExplorerContext(PathNavigator, notificationService, fileSystem, dialog);
        
        ToolBar = new ToolBarViewModel(context);
        
        contents = PathNavigator.Directories.Values()
            .Select(rooted => new DirectoryContentsViewModel(rooted, context))
            .DisposePrevious()
            .ToProperty(this, x => x.Contents);
    }

    public ToolBarViewModel ToolBar { get; }

    public DirectoryContentsViewModel Contents => contents.Value;
    public IFileSystem FileSystem { get; }
    public IPathNavigator PathNavigator { get; }
}