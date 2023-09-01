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

namespace Zafiro.Avalonia.FileExplorer.ViewModels;

public class DetailsViewModel : ViewModelBase
{
    private readonly IZafiroDirectory directory;
    private readonly SourceCache<IEntry, string> sourceCache;

    public DetailsViewModel(IZafiroDirectory directory)
    {
        this.directory = directory;
        sourceCache = new SourceCache<IEntry, string>(entry => entry.Path.Name());
        LoadChildren = ReactiveCommand.CreateFromTask(() => GetEntries(directory));

        LoadChildren.Successes().Do(entries => sourceCache.Edit(updater => updater.Load(entries))).Subscribe();

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