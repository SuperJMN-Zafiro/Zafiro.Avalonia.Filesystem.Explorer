using System;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls.Notifications;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.Pickers;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.Notifications;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class MainViewModel : ReactiveObject
{
    public MainViewModel(INotificationService notificationService)
    {
        var fileSystem = new SeaweedFileSystem(new SeaweedFSClient(new HttpClient() { BaseAddress = new Uri("http://192.168.1.31:8888") }), Maybe<ILogger>.None);
        
        ClipboardViewModel = new ClipboardViewModel();
        TransferManager = new TransferManagerViewModel();
        TransferManager
            .Transfers
            .ToObservableChangeSet()
            .OnItemAdded(r => r.DoTransfer.Start.Execute().Take(1).Subscribe())
            .Subscribe();

        Explorer = new ExplorerViewModel(fileSystem, DirectoryListing.GetAll, notificationService, ClipboardViewModel, TransferManager, notificationService);
        var picker = new FolderPicker(new DesktopDialogService(Maybe<Action<ConfigureWindowContext>>.None), fileSystem, notificationService, ClipboardViewModel, TransferManager);
        Pick = ReactiveCommand.CreateFromObservable(() => picker.Pick("Pick a folder"));
        Pick.Subscribe(maybe => { });
    }

    public TransferManagerViewModel TransferManager { get; set; }

    public ClipboardViewModel ClipboardViewModel { get; set; }

    public ReactiveCommand<Unit, Maybe<IZafiroDirectory>> Pick { get; set; }

    public ExplorerViewModel Explorer { get; }
}