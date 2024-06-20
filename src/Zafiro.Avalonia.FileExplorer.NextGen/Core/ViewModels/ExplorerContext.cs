using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Input.Platform;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.Controls;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.Misc;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public class ExplorerContext : IDisposable
{
    private readonly CompositeDisposable disposable = new();
    public IPathNavigator PathNavigator { get; }
    public INotificationService NotificationService { get; }
    public IFileSystem FileSystem { get; }
    public IDialog Dialog { get; }

    public ExplorerContext(IPathNavigator pathNavigator, INotificationService notificationService,
        IFileSystem fileSystem, IDialog dialog)
    {
        PathNavigator = pathNavigator;
        NotificationService = notificationService;
        FileSystem = fileSystem;
        Dialog = dialog;
        var directories = pathNavigator.Directories.Values().Select(rooted => new DirectoryContentsViewModel(rooted, this)).Publish();
        Directory = directories;
        directories.Select(x => x.Selection).Subscribe(model => { });
        SelectionContext = new SelectionContext(directories);
        directories.Connect().DisposeWith(disposable);
    }

    public SelectionContext SelectionContext { get; set; }

    public IObservable<DirectoryContentsViewModel> Directory { get; }
    
    [Reactive] public bool IsSelectionEnabled { get; set; }

    public void Dispose()
    {
        disposable.Dispose();
    }
}