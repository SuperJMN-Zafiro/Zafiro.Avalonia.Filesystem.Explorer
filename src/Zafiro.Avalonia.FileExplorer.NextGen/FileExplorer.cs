using Zafiro.Avalonia.FileExplorer.NextGen.Core;
using Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;
using Zafiro.FileSystem.DynamicData;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen;

public class FileExplorer
{
    public FileExplorer(IFileSystem fileSystem, INotificationService notificationService)
    {
        FileSystem = fileSystem;
        PathNavigator = new PathNavigatorViewModel(fileSystem, notificationService);
        PathNavigator.CurrentDirectory.Subscribe(maybe => { });
    }

    public IFileSystem FileSystem { get; }
    public IPathNavigator PathNavigator { get; }
}