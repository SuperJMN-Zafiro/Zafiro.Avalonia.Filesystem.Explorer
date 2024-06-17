using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.NextGen.Core;
using Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.DynamicData;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen;

public class FileExplorer : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<DirectoryContentsViewModel> contents;

    public FileExplorer(IFileSystem fileSystem, INotificationService notificationService)
    {
        FileSystem = fileSystem;
        PathNavigator = new PathNavigatorViewModel(fileSystem, notificationService);
        var context = new ExplorerContext(PathNavigator, notificationService);
        contents = PathNavigator.CurrentDirectory.Values()
            .Select(rooted => new DirectoryContentsViewModel(notificationService, rooted, context))
            .DisposePrevious()
            .ToProperty(this, x => x.Contents);
    }

    public DirectoryContentsViewModel Contents => contents.Value;
    public IFileSystem FileSystem { get; }
    public IPathNavigator PathNavigator { get; }
}

public class ExplorerContext
{
    public IPathNavigator PathNavigator { get; }

    public ExplorerContext(IPathNavigator pathNavigator, INotificationService notificationService)
    {
        PathNavigator = pathNavigator;
    }
}