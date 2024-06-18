using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.FileSystem.DynamicData;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core;

public class ExplorerContext
{
    public IPathNavigator PathNavigator { get; }
    public INotificationService NotificationService { get; }
    public IFileSystem FileSystem { get; }
    public IDialog Dialog { get; }

    public ExplorerContext(IPathNavigator pathNavigator, INotificationService notificationService,
        IFileSystem fileSystem, IDialog dialog)
    {
        PathNavigator = pathNavigator;
        NotificationService = notificationService;
        FileSystem = fileSystem;
        Dialog = dialog;
    }
}