using System;
using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer.NextGen;
using Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;
using Zafiro.FileSystem.DynamicData;
using Zafiro.UI;

namespace AvaloniaApplication1.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel(IFileSystem fileSystem, INotificationService notificationService, ISimpleDialog dialogService)
    {
        FileExplorer = new FileExplorer(fileSystem, notificationService, dialogService);
        //Observable.Timer(TimeSpan.FromSeconds(5), RxApp.MainThreadScheduler).SelectMany(l =>
        //{
        //    return Observable.FromAsync(() => dialogService.ShowMessage("hola, tío", "holas"));
        //}).Subscribe();
    }

    public FileExplorer FileExplorer { get; }
}