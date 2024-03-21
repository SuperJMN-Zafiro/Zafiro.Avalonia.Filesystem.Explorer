using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.TransferManager;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class ExplorerContext
{
    public ExplorerContext(INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager, IContentOpener opener)
    {
        NotificationService = notificationService;
        Clipboard = clipboard;
        TransferManager = transferManager;
        Opener = opener;
    }

    public INotificationService NotificationService { get; }
    public IClipboard Clipboard { get; }
    public ITransferManager TransferManager { get; }
    public IContentOpener Opener { get; }
}