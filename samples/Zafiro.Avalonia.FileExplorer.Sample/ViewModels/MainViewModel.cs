using System;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Http.Logging;
using ReactiveUI;
using Renci.SshNet;
using Serilog;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.FileSystem.Sftp;
using Zafiro.Misc;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class MainViewModel : ReactiveObject
{
    public MainViewModel(INotificationService notificationService)
    {
        var fileSystem = new FileSystemRoot(new ObservableFileSystem(Local()));

        ClipboardViewModel = new ClipboardViewModel();
        TransferManager = new TransferManagerViewModel { AutoStartOnAdd = true };
        var opener = new OperatingSystemContentOpener();
        FileSystemExplorer = new FileSystemExplorer(fileSystem, notificationService, ClipboardViewModel, TransferManager, opener);
        CurrentAddress = FileSystemExplorer.CurrentDirectory.Values().Select(path => path.ToString()!);
    }

    private static IZafiroFileSystem Local()
    {
        return LocalFileSystem.Create();
    }

    private static IZafiroFileSystem Sftp()
    {
        var sftpClient = new SftpClient("host", "usr", "password");
        sftpClient.Connect();
        return new SftpFileSystem(sftpClient);
    }

    private static IZafiroFileSystem SeaweedFileSystem()
    {
        var seaweedFSClient = new SeaweedFSClient(new HttpClient(new LoggingHttpMessageHandler(new LoggerAdapter(Log.Logger))
        {
            InnerHandler = new HttpClientHandler()
        })
        {
            BaseAddress = new Uri("http://192.168.1.29:8888"),
        });
        var seaweedFileSystem = new SeaweedFileSystem(seaweedFSClient, Maybe<ILogger>.None);
        return seaweedFileSystem;
    }

    public IObservable<string> CurrentAddress { get; }

    public TransferManagerViewModel TransferManager { get; }

    public ClipboardViewModel ClipboardViewModel { get; }

    public IFileSystemExplorer FileSystemExplorer { get; }
}