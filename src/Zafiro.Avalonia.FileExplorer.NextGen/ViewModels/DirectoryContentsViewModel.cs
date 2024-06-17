using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using DynamicData;
using ReactiveUI;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Mutable;
using Zafiro.UI;
using File = Zafiro.FileSystem.File;

namespace Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;

public class DirectoryContentsViewModel : ViewModelBase, IDisposable
{
    public IRooted<IMutableDirectory> Directory { get; }
    public ExplorerContext Context { get; }
    private readonly CompositeDisposable disposable = new();
    private readonly SourceCache<IEntry, string> entriesCache = new(x => x.Name);

    public DirectoryContentsViewModel(IRooted<IMutableDirectory> directory,
        ExplorerContext context)
    {
        Directory = directory;
        Context = context;
        Update().Tap(files => entriesCache.AddOrUpdate(files));
        
        
        entriesCache
            .Connect()
            .Bind(out var itemCollection)
            .Subscribe()
            .DisposeWith(disposable);

        Observable.Interval(TimeSpan.FromSeconds(5))
            .Do(_ =>
            {
                Update().Tap(files => entriesCache.EditDiff(files, (a, b) => Equals(a.Name, b.Name)));
            })
            .Subscribe()
            .DisposeWith(disposable);
        
        Items = itemCollection;
        CreateFile = ReactiveCommand.CreateFromTask(() => directory.Value.AddOrUpdate(new File("Random", "Content")));
        CreateFile
            .HandleErrorsWith(context.NotificationService)
            .DisposeWith(disposable);
    }
    
    private Task<Result<IEnumerable<IEntry>>> Update()
    { 
        var fileVms = Directory.Value.MutableFiles().MapEach(x => (IEntry)new FileViewModel(Directory, x));
        var dirVms = Directory.Value.MutableDirectories().MapEach(x => (IEntry)new DirectoryViewModel(Directory, x, Context));

        return dirVms.CombineAndMap(fileVms, (a, b) => a.Concat(b));
    }

    public ReadOnlyObservableCollection<IEntry> Items { get; set; }

    public ReactiveCommand<Unit,Result> CreateFile { get; set; }

    public void Dispose()
    {
        disposable.Dispose();
        CreateFile.Dispose();
    }
}