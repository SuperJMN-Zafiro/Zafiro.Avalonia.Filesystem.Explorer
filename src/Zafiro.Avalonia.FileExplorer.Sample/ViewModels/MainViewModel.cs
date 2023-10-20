using System;
using System.Net.Http;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;
using ILogger = Serilog.ILogger;
using Zafiro.Avalonia.FileExplorer.Model;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class MainViewModel : ReactiveObject
{
    public MainViewModel(INotificationService notificationService)
    {
        var fileSystem = new SeaweedFileSystem(new SeaweedFSClient(new HttpClient { BaseAddress = new Uri("http://192.168.1.29:8888") }), Maybe<ILogger>.None);
        
        ClipboardViewModel = new ClipboardViewModel();
        TransferManager = new TransferManagerViewModel { AutoStartOnAdd = true };
        FileSystemExplorer = new FileSystemExplorer(fileSystem, notificationService, ClipboardViewModel, TransferManager);
        FileSystemExplorer.GoTo(ZafiroPath.Empty);
        CurrentAddress = FileSystemExplorer.CurrentDirectory.Values().Select(path => path.ToString()!);
    }

    public IObservable<string> CurrentAddress { get; }

    public TransferManagerViewModel TransferManager { get; }

    public ClipboardViewModel ClipboardViewModel { get; }
    
    public IFileSystemExplorer FileSystemExplorer { get; }
}