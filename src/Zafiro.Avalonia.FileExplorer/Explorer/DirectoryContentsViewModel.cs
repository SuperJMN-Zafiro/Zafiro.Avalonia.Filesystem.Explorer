using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls.Selection;
using DynamicData;
using DynamicData.Binding;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.Misc;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class DirectoryContentsViewModel : ReactiveObject, IDisposable
{
    private readonly SourceCache<IEntry, string> contentsCache = new(entry => entry.Path.Name());
    private readonly IZafiroDirectory directory;
    private readonly CompositeDisposable disposable = new();
    private readonly INotificationService notificationService;
    private readonly IContentOpener opener;
    private readonly IPathNavigator pathNavigator;
    private readonly ISelectionContext selectionCommandses;

    public DirectoryContentsViewModel(IZafiroDirectory directory, IEntryFactory strategy, IPathNavigator pathNavigator, INotificationService notificationService, IContentOpener opener, ISelectionContext selectionCommandses)
    {
        this.directory = directory;
        this.pathNavigator = pathNavigator;
        this.notificationService = notificationService;
        this.opener = opener;
        this.selectionCommandses = selectionCommandses;
        LoadChildren = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await strategy.Get(directory);
            return result;
        }).DisposeWith(disposable);

        UpdateWhenContentsChange(directory)
            .DisposeWith(disposable);

        LoadChildren.Successes().Do(entries => contentsCache.Edit(updater => updater.Load(entries))).Subscribe();
        LoadChildren.HandleErrorsWith(notificationService);

        var observable = contentsCache
            .Connect();

        observable
            .Sort(SortExpressionComparer<IEntry>.Descending(p => p is DirectoryItemViewModel)
                .ThenByAscending(p => p.Path.Name()))
            .Bind(out var collection)
            .Subscribe()
            .DisposeWith(disposable);

        Children = collection;
        IsLoadingChildren = LoadChildren.IsExecuting.DelayItem(true, TimeSpan.FromSeconds(0.5), RxApp.MainThreadScheduler);
        LoadChildren.Execute().Subscribe().DisposeWith(disposable);
        var tracker = new SelectionTracker<IEntry, ZafiroPath>(Selection, entry => entry.Path);
        tracker.Changes.Bind(out var selectedItems).Subscribe().DisposeWith(disposable);
        SelectedItems = selectedItems;
        Paste = selectionCommandses.Paste;
        SelectionContext = selectionCommandses;
    }

    public ISelectionContext SelectionContext { get; }

    public ReactiveCommand<Unit, IAction<LongProgress>> Paste { get; }

    public ReadOnlyObservableCollection<IEntry> SelectedItems { get; }

    public IObservable<bool> IsLoadingChildren { get; }

    public ReactiveCommand<Unit, Result<IEnumerable<IEntry>>> LoadChildren { get; }

    public ZafiroPath Path => directory.Path;

    public ReadOnlyObservableCollection<IEntry> Children { get; }

    public SelectionModel<IEntry> Selection { get; } = new() { SingleSelect = false };

    public void Dispose()
    {
        disposable.Dispose();
    }

    private IDisposable UpdateWhenContentsChange(IZafiroDirectory directory) => directory.Changed
        .Select(UpdateFrom)
        .Subscribe();

    private async Task UpdateFrom(FileSystemChange change)
    {
        if (change.Change == Change.FileCreated)
        {
            var file = directory.FileSystem.GetFile(change.Path);
            await file.Properties
                .Map(properties => new FileItemViewModel(file, properties, opener, selectionCommandses, notificationService))
                .Tap(f => contentsCache.AddOrUpdate(f));
        }

        if (change.Change == Change.DirectoryCreated)
        {
            var dir = directory.FileSystem.GetDirectory(change.Path);
            contentsCache.AddOrUpdate(new DirectoryItemViewModel(dir, pathNavigator));
        }

        if (change.Change == Change.FileDeleted || change.Change == Change.DirectoryDeleted)
        {
            contentsCache.RemoveKey(change.Path.Name());
        }
    }
}