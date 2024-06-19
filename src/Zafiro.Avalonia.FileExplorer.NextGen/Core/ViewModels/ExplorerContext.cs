using System.Reactive.Disposables;
using System.Reactive.Linq;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;
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
        directories.Connect().DisposeWith(disposable);
    }

    public IObservable<DirectoryContentsViewModel> Directory { get; }

    public void Dispose()
    {
        disposable.Dispose();
    }
}