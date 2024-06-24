using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Clipboard;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public class ExplorerContext : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable disposable = new();
    public IPathNavigator PathNavigator { get; }
    public INotificationService NotificationService { get; }
    public IMutableFileSystem MutableFileSystem { get; }
    public IDialog Dialog { get; }

    public ExplorerContext(IPathNavigator pathNavigator, INotificationService notificationService,
        IMutableFileSystem mutableFileSystem, IDialog dialog, IClipboardService clipboardService)
    {
        PathNavigator = pathNavigator;
        NotificationService = notificationService;
        MutableFileSystem = mutableFileSystem;
        Dialog = dialog;
        var directories = pathNavigator.Directories.Values().Select(rooted => new DirectoryContentsViewModel(rooted, this)).ReplayLastActive();
        Directory = directories;
        SelectionContext = new SelectionContext(directories);

        SelectionContext.SelectionChanges.Bind(out var selectedItems).Subscribe().DisposeWith(disposable);
        
        Copy = directories.Select(d => ReactiveCommand.CreateFromTask(() =>
        {
            var copy = clipboardService.Copy(selectedItems, d.Directory.Path, MutableFileSystem);
            copy.Tap(() => notificationService.Show("Copied"));
            return copy;
        }));
        
        Paste = directories.Select(d => ReactiveCommand.CreateFromTask(() => clipboardService.Paste(d.Directory.Value)));
        Delete = ReactiveCommand.CreateFromTask(async () =>
        {
            var deletes = selectedItems.Select(item => item.Delete);
            IEnumerable<IObservable<Result>> executes = deletes.Select(x => x.Execute());
            var merged = executes.Merge();
            var list = await merged.ToList();
            return list.Combine();
        });
    }
    
    public IObservable<ReactiveCommand<Unit, Result>> Paste { get; }

    public IObservable<ReactiveCommand<Unit, Result>> Copy { get; }

    public SelectionContext SelectionContext { get; set; }

    public IObservable<DirectoryContentsViewModel> Directory { get; }

    [Reactive]
    public bool IsSelectionEnabled { get; set; }

    public ReactiveCommand<Unit, Result> Delete { get; }

    public void Dispose()
    {
        disposable.Dispose();
    }
}