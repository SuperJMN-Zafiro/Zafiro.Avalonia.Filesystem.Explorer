using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;
using static Zafiro.Avalonia.FileExplorer.Model.DirectoryListing;

namespace Zafiro.Avalonia.FileExplorer.ViewsModes.FolderContents;

public class DetailsViewModel : ReactiveObject
{
    private readonly IZafiroDirectory directory;

    public DetailsViewModel(IZafiroDirectory directory, Strategy strategy, INotificationService notificationService, IPendingActionsManager pendingActions)
    {
        this.directory = directory;
        SourceCache<IEntry, string> sourceCache = new(entry => entry.Path.Name());
        LoadChildren = ReactiveCommand.CreateFromTask(() => strategy(directory));

        LoadChildren.Successes().Do(entries => sourceCache.Edit(updater => updater.Load(entries))).Subscribe();
        LoadChildren.HandleErrorsWith(notificationService);

        var observable = sourceCache
            .Connect();

        observable
            .Sort(SortExpressionComparer<IEntry>.Descending(p => p is FolderItemViewModel)
                .ThenByAscending(p => p.Path.Name()))
            .Bind(out var collection)
            .Subscribe();

        observable
            .AutoRefresh(x => x.IsSelected)
            .Filter(x => x.IsSelected)
            .Bind(out var selectedItems)
            .Subscribe();

        HasSelection = observable
            .AutoRefresh(x => x.IsSelected)
            .Filter(x => x.IsSelected)
            .ToCollection()
            .Select(x => x.Any())
            .StartWith(false);
            
        SelectedItems = selectedItems;

        Children = collection;
        IsLoadingChildren = LoadChildren.IsExecuting.DelayItem(true, TimeSpan.FromSeconds(0.5));
        LoadChildren.Execute().Subscribe();
        Copy = ReactiveCommand.Create(() =>
        {
            var clipboardItems = selectedItems.Select(entry =>
            {
                return entry switch
                {
                    FolderItemViewModel di => (IClipboardItem)new ClipboardDirectoryItemViewModel(di.Directory),
                    FileItemViewModel fi => new ClipboardFileItemViewModel(fi.File),
                    _ => throw new ArgumentOutOfRangeException(nameof(entry))
                };
            });

            pendingActions.Copy(clipboardItems);
        });
    }

    public IObservable<bool> HasSelection { get; }

    public ReadOnlyObservableCollection<IEntry> SelectedItems { get; }

    public IObservable<bool> IsLoadingChildren { get; }

    public ReactiveCommand<Unit, Result<IEnumerable<IEntry>>> LoadChildren { get; }

    public ZafiroPath Path => directory.Path;

    public ReadOnlyObservableCollection<IEntry> Children { get; }

    [Reactive] public IEntry SelectedItem { get; set; }

    public string Name => Path.Name();
    public ICommand Copy { get; }
}