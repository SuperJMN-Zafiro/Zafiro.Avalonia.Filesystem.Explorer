using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls.Selection;
using DynamicData;
using DynamicData.Binding;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.Mixins;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;

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

        Entries = directory.Value.Children.Transform(node =>
        {
            return node switch
            {
                IMutableDirectory mutableDirectory => (IDirectoryItem) new DirectoryViewModel(Directory, mutableDirectory, context),
                IMutableFile mutableFile => new FileViewModel(directory.Value, mutableFile),
                _ => throw new ArgumentOutOfRangeException(nameof(node))
            };
        });

        Entries
            .Sort(SortExpressionComparer<IDirectoryItem>.Descending(p => p is DirectoryViewModel)
                .ThenByAscending(p => p.Name))
            .Bind(out var itemsCollection)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposable);

        Items = itemsCollection;

       

        Entries
            .Transform(item => item.Deleted.Do(_ => entriesCache.Remove(item)).Subscribe())
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposable);
    }

    public IObservable<IChangeSet<IDirectoryItem,string>> Entries { get; }

    public ReadOnlyObservableCollection<IDirectoryItem> Items { get; }

    public SelectionModel<IDirectoryItem> Selection { get; } = new() { SingleSelect = false };

    public void Dispose()
    {
        disposable.Dispose();
    }

    public Task<Result<IMutableDirectory>> CreateDirectory(string name)
    {
        return Directory.Value.CreateSubdirectory(name)
            .Tap(dir =>
            {
                entriesCache.AddOrUpdate(new DirectoryViewModel(Directory, dir, Context));
            });
    }
}