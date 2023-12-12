using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls.Selection;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.Mixins;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class DirectoryContentsViewModel : ReactiveObject, IDisposable
{
    private readonly IZafiroDirectory directory;
    private readonly SourceCache<IEntry, string> contentsCache = new(entry => entry.Path.Name());
    private readonly CompositeDisposable disposable = new();

    public DirectoryContentsViewModel(IZafiroDirectory directory, IEntryFactory strategy, INotificationService notificationService, IClipboard pendingActions, ITransferManager downloadManager)
    {
        this.directory = directory;
        LoadChildren = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await strategy.Get(directory);
            return result;
        }).DisposeWith(disposable);

        UpdateWhenContentsChange(directory)
            .DisposeWith(disposable);

        LoadChildren.Successes().Do(entries => contentsCache.Edit(updater => updater.Load(entries))).Subscribe();
        LoadChildren.HandleErrorsWith(notificationService);

        var observable = contentsCache
            .Connect();

        observable
            .Sort(SortExpressionComparer<IEntry>.Descending(p => p is DirectoryItemViewModel)
                .ThenByAscending(p => p.Path.Name()))
            .Bind(out var collection)
            .Subscribe()
            .DisposeWith(disposable);

        Children = collection;
        IsLoadingChildren = LoadChildren.IsExecuting.DelayItem(true, TimeSpan.FromSeconds(0.5), RxApp.MainThreadScheduler);
        LoadChildren.Execute().Subscribe().DisposeWith(disposable);
        var tracker = new SelectionTracker<IEntry, ZafiroPath>(Selection, entry => entry.Path);
        tracker.Changes.Bind(out var selectedItems).Subscribe().DisposeWith(disposable);
        SelectedItems = selectedItems;
    }

    private IDisposable UpdateWhenContentsChange(IZafiroDirectory directory) => directory.Changed
        .Do(UpdateFrom)
        .Subscribe();

    private void UpdateFrom(FileSystemChange change)
    {
        var file = directory.FileSystem.GetFile(change.Path);
        if (change.Change == Change.FileCreated)
        {
            contentsCache.AddOrUpdate(new FileItemViewModel(file));
        }
        if (change.Change == Change.FileDeleted)
        {
            contentsCache.RemoveKey(file.Path.Name());
        }
    }

    public ReadOnlyObservableCollection<IEntry> SelectedItems { get; }

    public IObservable<bool> IsLoadingChildren { get; }

    public ReactiveCommand<Unit, Result<IEnumerable<IEntry>>> LoadChildren { get; }

    public ZafiroPath Path => directory.Path;

    public ReadOnlyObservableCollection<IEntry> Children { get; }

    public SelectionModel<IEntry> Selection { get; } = new() {  SingleSelect = false };

    public void Dispose()
    {
        disposable.Dispose();
    }
}