using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.ViewsModes.Tree;

public class TreeViewModel : ReactiveObject
{
    private readonly IZafiroDirectory directory;
    private readonly SourceCache<IEntry, string> sourceCache;

    public TreeViewModel(IZafiroDirectory directory)
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
        IsLoadingChildren = LoadChildren.IsExecuting;
    }

    public IObservable<bool> IsLoadingChildren { get; }

    public ReactiveCommand<Unit, Result<IEnumerable<IEntry>>> LoadChildren { get; }

    public ZafiroPath Path => directory.Path;

    public ReadOnlyObservableCollection<IEntry> Children { get; }
    public string Name => Path.Name();

    public async Task<Result<IEntry>> Add(string name, Stream contents, CancellationToken cancellationToken)
    {
        var result = await directory.GetFile(name)
            .Map(file => new FileItemViewModel(file))
            .Tap(f => sourceCache.AddOrUpdate(f))
            .Map(file => (IEntry)file);

        return result;
    }

    private Task<Result<IEnumerable<IEntry>>> GetEntries(IZafiroDirectory directory)
    {
        var files = directory.GetFiles().Map(files => files.Select(file => (IEntry)new FileItemViewModel(file)));
        var dirs = directory.GetDirectories().Map(dirs => dirs.Select(dir => (IEntry)new TreeViewModel(dir)));

        return from f in files
               from n in dirs
               select f.Concat(n);
    }
}