using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Alias;
using ReactiveUI;
using Zafiro.FileSystem.DynamicData;
using Zafiro.UI;
using File = Zafiro.FileSystem.File;

namespace Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;

public class DirectoryContentsViewModel : ViewModelBase, IDisposable
{
    private readonly CompositeDisposable disposable = new();
    
    public DirectoryContentsViewModel(INotificationService notificationService, IDynamicDirectory directory)
    {
        var fileVms = directory.Files.Select(x => (IEntry)new FileViewModel(directory, x));
        var dirVms = directory.Directories.Select(x => (IEntry)new DirectoryViewModel(directory, x));

        var entries = fileVms.MergeChangeSets(dirVms);
        
        entries
            .Bind(out var itemCollection)
            .Subscribe()
            .DisposeWith(disposable);
        
        Items = itemCollection;
        CreateFile = ReactiveCommand.CreateFromTask(() => directory.AddOrUpdateFile(new File("Random", "Content")));
        CreateFile
            .HandleErrorsWith(notificationService)
            .DisposeWith(disposable);
    }

    public ReadOnlyObservableCollection<IEntry> Items { get; set; }

    public ReactiveCommand<Unit,Result> CreateFile { get; set; }

    public void Dispose()
    {
        disposable.Dispose();
        CreateFile.Dispose();
    }
}