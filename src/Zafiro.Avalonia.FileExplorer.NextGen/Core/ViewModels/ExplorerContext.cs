using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.FileSystem.DynamicData;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core;

public class ExplorerContext
{
    public IPathNavigator PathNavigator { get; }
    public INotificationService NotificationService { get; }
    public IFileSystem FileSystem { get; }
    public ISimpleDialog Dialog { get; }

    public ExplorerContext(IPathNavigator pathNavigator, INotificationService notificationService,
        IFileSystem fileSystem, ISimpleDialog dialog)
    {
        PathNavigator = pathNavigator;
        NotificationService = notificationService;
        FileSystem = fileSystem;
        Dialog = dialog;
    }
}