using System.Collections.Generic;
using System.Linq;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer;
using Zafiro.Avalonia.FileExplorer.Core.Clipboard;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.UI;

namespace SampleFileExplorer.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(List<Plugin> mutableFileSystem, INotificationService notificationService, IDialog dialogService,
        IClipboardService clipboardService, ITransferManager transferManager)
    {
        Explorers = mutableFileSystem.Select(plugin => new FileExplorer(plugin.FileSystem, notificationService, dialogService, clipboardService, transferManager));
    }

    public IEnumerable<FileExplorer> Explorers { get; }
}