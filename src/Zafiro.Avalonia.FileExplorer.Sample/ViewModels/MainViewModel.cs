using System;
using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Local;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class MainViewModel : ReactiveObject
{
    public MainViewModel(INotificationService notificationService)
    {
        var fileSystem = new FileSystemRoot(new ObservableFileSystem(LocalFileSystem.Create()));
        
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