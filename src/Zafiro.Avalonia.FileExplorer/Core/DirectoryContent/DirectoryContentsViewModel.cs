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

    public DirectoryContentsViewModel(IRooted<IMutableDirectory> directory,
        ExplorerContext context)
    {
        Directory = directory;
        Context = context;
        
        directory.Value.Children
            .DisposeMany()
            .Transform(DirectoryItem)
            .Sort(SortExpressionComparer<IDirectoryItem>.Descending(p => p is DirectoryViewModel)
                .ThenByAscending(p => p.Name))
            .Bind(out var itemsCollection)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposable);

        Items = itemsCollection;
    }

    private IDirectoryItem DirectoryItem(IMutableNode node)
    {
        return node switch
        {
            IMutableDirectory mutableDirectory => new DirectoryViewModel(Directory, mutableDirectory, Context),
            IMutableFile mutableFile => new FileViewModel(Directory.Value, mutableFile),
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };
    }

    public IObservable<IChangeSet<IDirectoryItem,string>> Entries { get; }

    public ReadOnlyObservableCollection<IDirectoryItem> Items { get; }

    public SelectionModel<IDirectoryItem> Selection { get; } = new() { SingleSelect = false };

    public void Dispose()
    {
        disposable.Dispose();
    }
}