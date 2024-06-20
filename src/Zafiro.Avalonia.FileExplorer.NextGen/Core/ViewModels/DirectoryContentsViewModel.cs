using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls.Selection;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.Mixins;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public class DirectoryContentsViewModel : ViewModelBase, IDisposable
{
    public IRooted<IMutableDirectory> Directory { get; }
    public ExplorerContext Context { get; }
    private readonly CompositeDisposable disposable = new();
    private readonly SourceCache<IDirectoryItem, string> entriesCache = new(x => x.Name);

    public DirectoryContentsViewModel(IRooted<IMutableDirectory> directory,
        ExplorerContext context)
    {
        Directory = directory;
        Context = context;
        Update().Tap(files => entriesCache.AddOrUpdate(files));

        var entries = entriesCache
            .Connect();

        entries
            .Sort(SortExpressionComparer<IDirectoryItem>.Descending(p => p is DirectoryViewModel)
                .ThenByAscending(p => p.Name))
            .Bind(out var itemsCollection)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposable);

        entries.Subscribe(set => { Debug.WriteLine(set.JoinWithLines()); });

        entries
            .Transform(item => item.Deleted.Do(_ => entriesCache.Remove(item)).Subscribe())
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposable);

        // Observable.Interval(TimeSpan.FromSeconds(5))
        //     .Do(_ => { Update().Tap(entries => entriesCache.EditDiff(entries, (a, b) => Equals(a.Key, b.Key))); })
        //     .Subscribe()
        //     .DisposeWith(disposable);

        Items = itemsCollection;

        Selection.SelectionChanged += (sender, args) => { };
    }

    private Task<Result<IEnumerable<IDirectoryItem>>> Update()
    {
        var fileVms = Directory.Value.MutableFiles().Map(files => files.Where(file => !file.IsHidden))
            .MapEach(x => (IDirectoryItem)new FileViewModel(Directory, x));
        var dirVms = Directory.Value.MutableDirectories().Map(files => files.Where(file => !file.IsHidden))
            .MapEach(x => (IDirectoryItem)new DirectoryViewModel(Directory, x, Context));

        return dirVms.CombineAndMap(fileVms, (a, b) => a.Concat(b));
    }

    public ReadOnlyObservableCollection<IDirectoryItem> Items { get; set; }

    public SelectionModel<IDirectoryItem> Selection { get; } = new() { SingleSelect = false };

    public void Dispose()
    {
        disposable.Dispose();
    }

    public Task<Result<IMutableDirectory>> CreateDirectory(string name)
    {
        return Directory.Value.CreateDirectory(name)
            .Tap(dir =>
            {
                entriesCache.AddOrUpdate(new DirectoryViewModel(Directory, dir, Context));
            });
    }
}