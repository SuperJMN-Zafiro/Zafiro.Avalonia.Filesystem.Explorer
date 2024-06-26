using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer.Core.Clipboard;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.Avalonia.FileExplorer.Core.Navigator;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Core;

public class ExplorerContext : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable disposable = new();
    public IPathNavigator PathNavigator { get; }
    public ITransferManager TransferManager { get; }
    public INotificationService NotificationService { get; }
    public IMutableFileSystem MutableFileSystem { get; }
    public IDialog Dialog { get; }

    public ExplorerContext(IPathNavigator pathNavigator, ITransferManager transferManager,
        INotificationService notificationService,
        IMutableFileSystem mutableFileSystem, IDialog dialog, IClipboardService clipboardService)
    {
        PathNavigator = pathNavigator;
        TransferManager = transferManager;
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
        }, SelectionContext.SelectionCount.Select(i => i > 0)));
        
        Paste = directories.Select(d => ReactiveCommand.CreateFromTask(() => clipboardService.Paste(d.Directory.Value), clipboardService.CanPaste)).ReplayLastActive();
        
        Delete = directories.Select(d => ReactiveCommand.CreateFromTask(async () =>
        {
            var confirm = await Dialog.ShowConfirmation($"Delete", $"Do you really want to delete the selected items?");
            if (confirm)
            {
                var deletes = selectedItems.Select(item => item.Delete).ToList();
                var executes = deletes.Select(x => x.Execute());
                var executionResults = await executes.Merge().ToList();
                return executionResults.Combine();
            }

            return Result.Success();
        }, SelectionContext.SelectionCount.Select(i => i > 0)));
    }

    public IObservable<ReactiveCommand<Unit, Result>> Paste { get; }

    public IObservable<ReactiveCommand<Unit, Result>> Copy { get; }

    public SelectionContext SelectionContext { get; set; }

    public IObservable<DirectoryContentsViewModel> Directory { get; }

    [Reactive]
    public bool IsSelectionEnabled { get; set; }

    public IObservable<ReactiveCommand<Unit, Result>> Delete { get; }

    public void Dispose()
    {
        disposable.Dispose();
    }
}