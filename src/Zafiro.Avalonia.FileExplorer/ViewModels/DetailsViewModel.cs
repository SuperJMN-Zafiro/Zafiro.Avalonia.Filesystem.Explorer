using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;
using static Zafiro.Avalonia.FileExplorer.ViewModels.DirectoryListing;

namespace Zafiro.Avalonia.FileExplorer.ViewModels;

public class DetailsViewModel : ViewModelBase
{
    private readonly IZafiroDirectory directory;

    public DetailsViewModel(IZafiroDirectory directory, Strategy strategy, INotificationService notificationService)
    {
        this.directory = directory;
        SourceCache<IEntry, string> sourceCache = new(entry => entry.Path.Name());
        LoadChildren = ReactiveCommand.CreateFromTask(() => strategy(directory));

        LoadChildren.Successes().Do(entries => sourceCache.Edit(updater => updater.Load(entries))).Subscribe();
        LoadChildren.HandleErrorsWith(notificationService);

        sourceCache
            .Connect()
            .Sort(SortExpressionComparer<IEntry>.Descending(p => p is FolderItemViewModel)
                .ThenByAscending(p => p.Path.Name()))
            .Bind(out var collection)
            .Subscribe();

        Children = collection;
        IsLoadingChildren = LoadChildren.IsExecuting.DelayItem(true, TimeSpan.FromSeconds(0.5));
        LoadChildren.Execute().Subscribe();
        SelectedItems = this.WhenAnyValue(x => x.SelectedItem);
    }

    public IObservable<IEntry> SelectedItems { get; }

    public IObservable<bool> IsLoadingChildren { get; }

    public ReactiveCommand<Unit, Result<IEnumerable<IEntry>>> LoadChildren { get; }

    public ZafiroPath Path => directory.Path;

    public ReadOnlyObservableCollection<IEntry> Children { get; }

    [Reactive] public IEntry SelectedItem { get; set; }

    public string Name => Path.Name();
}