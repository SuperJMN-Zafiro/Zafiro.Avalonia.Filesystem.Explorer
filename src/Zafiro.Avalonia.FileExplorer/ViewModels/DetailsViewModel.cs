using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
            .Sort(SortExpressionComparer<IEntry>.Descending(p => p is IFolder)
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

    private Task<Result<IEnumerable<IEntry>>> GetEntries(IZafiroDirectory directory)
    {
        var files = directory.GetFiles().Map(files => files.Select(file => (IEntry)new FileViewModel(file)));
        var dirs = directory.GetDirectories().Map(dirs => dirs.Select(dir => (IEntry)new FolderItemViewModel(dir)));

        return from f in files
               from n in dirs
               select f.Concat(n);
    }
}

public static class ObservableExtensions
{
    public static IObservable<T> DelayItem<T>(this IObservable<T> sequence, T itemToDelay, TimeSpan timeSpan) where T : notnull
    {
        return sequence
            .Select(x => x.Equals(itemToDelay) ? Observable.Return(x).Delay(timeSpan) : Observable.Return(x))
            .Switch();
    }
}