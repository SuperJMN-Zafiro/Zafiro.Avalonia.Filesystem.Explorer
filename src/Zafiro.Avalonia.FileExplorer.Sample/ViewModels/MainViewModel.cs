using System;
using System.Net.Http;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Http.Logging;
using ReactiveUI;
using Serilog;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.Misc;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class MainViewModel : ReactiveObject
{
    public MainViewModel(INotificationService notificationService)
    {
        var seaweedFSClient = new SeaweedFSClient(new HttpClient(new LoggingHttpMessageHandler(new LoggerAdapter(Log.Logger))
        {
            InnerHandler = new HttpClientHandler()
        })
        {
            BaseAddress = new Uri("http://192.168.1.29:8888"),
        });
        var fileSystem = new FileSystemRoot(new ObservableFileSystem(new SeaweedFileSystem(seaweedFSClient, Maybe<ILogger>.None)));
        
        ClipboardViewModel = new ClipboardViewModel();
        TransferManager = new TransferManagerViewModel { AutoStartOnAdd = true };
        FileSystemExplorer = new FileSystemExplorer(fileSystem, notificationService, ClipboardViewModel, TransferManager);
        CurrentAddress = FileSystemExplorer.CurrentDirectory.Values().Select(path => path.ToString()!);
    }

    public IObservable<string> CurrentAddress { get; }

    public TransferManagerViewModel TransferManager { get; }

    public ClipboardViewModel ClipboardViewModel { get; }
    
    public IFileSystemExplorer FileSystemExplorer { get; }
}