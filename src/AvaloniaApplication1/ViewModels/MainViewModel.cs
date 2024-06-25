using System;
using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer.NextGen;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Clipboard;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Mutable;
using Zafiro.UI;

namespace AvaloniaApplication1.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(IMutableFileSystem mutableFileSystem, INotificationService notificationService, IDialog dialogService,
        IClipboardService clipboardService, ITransferManager transferManager)
    {
        FileExplorer = new FileExplorer(mutableFileSystem, notificationService, dialogService, clipboardService, transferManager);
    }

    public FileExplorer FileExplorer { get; }
}