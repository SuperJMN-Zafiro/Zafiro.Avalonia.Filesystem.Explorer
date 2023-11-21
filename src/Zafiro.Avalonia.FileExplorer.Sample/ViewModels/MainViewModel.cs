using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;
using ILogger = Serilog.ILogger;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem.Local;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class MainViewModel : ReactiveObject
{
    public MainViewModel(INotificationService notificationService)
    {
        var fileSystem = new LocalFileSystem(new System.IO.Abstractions.FileSystem(), Maybe<ILogger>.None);
        
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