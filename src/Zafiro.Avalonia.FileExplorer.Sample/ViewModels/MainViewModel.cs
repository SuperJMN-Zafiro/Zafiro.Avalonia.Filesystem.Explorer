using System;
using System.Net.Http;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Pickers;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class MainViewModel : ReactiveObject
{
    public MainViewModel(INotificationService notificationService)
    {
        var fileSystem = new SeaweedFileSystem(new SeaweedFSClient(new HttpClient { BaseAddress = new Uri("http://192.168.1.29:8888") }), Maybe<ILogger>.None);
        
        ClipboardViewModel = new ClipboardViewModel();
        TransferManager = new TransferManagerViewModel { AutoStartOnAdd = true };
        FileSystemExplorer = new FileSystemExplorer(fileSystem, notificationService, ClipboardViewModel, TransferManager);
        var picker = new FolderPicker(new DesktopDialogService(Maybe<Action<ConfigureWindowContext>>.None), fileSystem, notificationService, ClipboardViewModel, TransferManager);
        Pick = ReactiveCommand.CreateFromObservable(() => picker.Pick("Pick a folder"));
    }

    public TransferManagerViewModel TransferManager { get; }

    public ClipboardViewModel ClipboardViewModel { get; }

    public ReactiveCommand<Unit, Maybe<IZafiroDirectory>> Pick { get; set; }

    public IFileSystemExplorer FileSystemExplorer { get; }
}