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
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.ViewModels;

public class FolderViewModel : ViewModelBase, IFolder
{
    private readonly IZafiroDirectory directory;
    private readonly SourceCache<IEntry, string> sourceCache;

    public FolderViewModel(IZafiroDirectory directory)
    {
        this.directory = directory;
        sourceCache = new SourceCache<IEntry, string>(entry => entry.Path.Name());
        Load = ReactiveCommand.CreateFromTask(() => GetEntries(directory));
        Load.Failures().Subscribe(s => { });
        
        Load.Successes().Do(entries => sourceCache.Edit(updater => updater.Load(entries))).Subscribe();

        sourceCache
            .Connect()
            .Sort(SortExpressionComparer<IEntry>.Descending(p => p is IFolder)
                .ThenByAscending(p => p.Path.Name()))
            .Bind(out var collection)
            .Subscribe();

        Children = collection;
    }

    public ReactiveCommand<Unit, Result<IEnumerable<IEntry>>> Load { get; }


    public ZafiroPath Path => directory.Path;

    public ReadOnlyObservableCollection<IEntry> Children { get; }
    public string Name => Path.Name();

    public async Task<Result<IEntry>> Add(string name, Stream contents, CancellationToken cancellationToken)
    {
        var result = await directory.GetFile(name)
            .Map(file => new FileViewModel(file))
            .Tap(f => sourceCache.AddOrUpdate(f))
            .Map(file => (IEntry)file);

        return result;
    }

    //public Task<Result> CreateFolder(string name)
    //{
    //    return Result
    //        .Try(() => seaweed.CreateFolder(PathUtils.Combine(Path, name)))
    //        .Tap(() => sourceCache.AddOrUpdate(new Folder(new FolderDto { Path = PathUtils.Combine(Path, name) }, seaweed)));
    //}

    //public Task<Result> DeleteFolder(string name)
    //{
    //    return Result
    //        .Try(() => throw new NotImplementedException())
    //        .Tap(() => sourceCache.Remove(name));
    //}

    //public Task<Result<IFolder>> Create(string path, ISeaweedFS seaweedFs)
    //{
    //    return Result
    //        .Try(() => directory.GetFile())
    //        .Map(folder => (IFolder) new Folder(folder, seaweedFs));
    //}

    private Task<Result<IEnumerable<IEntry>>> GetEntries(IZafiroDirectory directory)
    {
        var files = directory.GetFiles().Map(files => files.Select(file => (IEntry)new FileViewModel(file)));
        var dirs = directory.GetDirectories().Map(dirs => dirs.Select(dir => (IEntry)new FolderViewModel(dir)));

        return from f in files
            from n in dirs
            select f.Concat(n);
    }
}