﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls.Selection;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class DetailsViewModel : ReactiveObject
{
    private readonly IZafiroDirectory directory;

    public DetailsViewModel(IZafiroDirectory directory, IEntryFactory strategy, INotificationService notificationService, IClipboard pendingActions, ITransferManager downloadManager)
    {
        this.directory = directory;
        SourceCache<IEntry, string> entryCache = new(entry => entry.Path.Name());
        LoadChildren = ReactiveCommand.CreateFromTask(() => strategy.Get(directory));

        LoadChildren.Successes().Do(entries => entryCache.Edit(updater => updater.Load(entries))).Subscribe();
        LoadChildren.HandleErrorsWith(notificationService);

        var observable = entryCache
            .Connect();

        observable
            .Sort(SortExpressionComparer<IEntry>.Descending(p => p is DirectoryItemViewModel)
                .ThenByAscending(p => p.Path.Name()))
            .Bind(out var collection)
            .Subscribe();

        Children = collection;
        IsLoadingChildren = LoadChildren.IsExecuting.DelayItem(true, TimeSpan.FromSeconds(0.5));
        LoadChildren.Execute().Subscribe();
        var changes = Selection.ToObservable(x => x.Path);
        changes.Bind(out var selectedItems).Subscribe();
        SelectedItems = selectedItems;
    }

    public ReadOnlyObservableCollection<IEntry> SelectedItems { get; }

    public IObservable<bool> IsLoadingChildren { get; }

    public ReactiveCommand<Unit, Result<IEnumerable<IEntry>>> LoadChildren { get; }

    public ZafiroPath Path => directory.Path;

    public ReadOnlyObservableCollection<IEntry> Children { get; }

    [Reactive] public IEntry SelectedItem { get; set; }

    public SelectionModel<IEntry> Selection { get; } = new() { SingleSelect = false };
}