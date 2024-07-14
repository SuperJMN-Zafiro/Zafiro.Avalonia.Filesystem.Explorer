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
        LoadContents = ReactiveCommand.CreateFromTask(Update);
        LoadContents.HandleErrorsWith(context.NotificationService).DisposeWith(disposable);
        LoadContents.Successes().ObserveOn(RxApp.MainThreadScheduler).Subscribe(entries => entriesCache.AddOrUpdate(entries)).DisposeWith(disposable);
        Directory = directory;
        Context = context;

        Entries = entriesCache
            .Connect();

        Entries
            .Sort(SortExpressionComparer<IDirectoryItem>.Descending(p => p is DirectoryViewModel)
                .ThenByAscending(p => p.Name))
            .Bind(out var itemsCollection)
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposable);

        Items = itemsCollection;

        Entries.Subscribe(set => { Debug.WriteLine(set.JoinWithLines()); });

        Entries
            .Transform(item => item.Deleted.Do(_ => entriesCache.Remove(item)).Subscribe())
            .DisposeMany()
            .Subscribe()
            .DisposeWith(disposable);

        // Observable.Timer(TimeSpan.FromSeconds(5))
        //     .Do(_ => { Update().Tap(items => entriesCache.EditDiff(items, (a, b) => Equals(a.Key, b.Key))); })
        //     .Repeat()
        //     .Subscribe()
        //     .DisposeWith(disposable);
        IsBusy = LoadContents.IsExecuting;
    }

    public IObservable<bool> IsBusy { get; }

    public ReactiveCommand<Unit,Result<IEnumerable<IDirectoryItem>>> LoadContents { get; }

    public IObservable<IChangeSet<IDirectoryItem,string>> Entries { get; }

    private async Task<Result<IEnumerable<IDirectoryItem>>> Update()
    {
        var fileVms = (await Directory.Value.MutableFilesObs().Map(files => files.Where(file => !file.IsHidden)))
            .ManyMap(x => (IDirectoryItem)new FileViewModel(x));
        var dirVms = (await Directory.Value.MutableDirectoriesObs().Map(files => files.Where(file => !file.IsHidden)))
            .ManyMap(x => (IDirectoryItem)new DirectoryViewModel(Directory, x, Context));

        return dirVms.CombineAndMap(fileVms, (a, b) => a.Concat(b));
    }

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