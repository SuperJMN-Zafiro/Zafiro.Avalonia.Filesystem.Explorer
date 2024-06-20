using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using Avalonia.Input;
using Avalonia.Input.Platform;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.Mixins;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public class ExplorerContext : ReactiveObject, IDisposable
{
    private readonly CompositeDisposable disposable = new();
    public IPathNavigator PathNavigator { get; }
    public INotificationService NotificationService { get; }
    public IFileSystem FileSystem { get; }
    public IDialog Dialog { get; }

    public ExplorerContext(IPathNavigator pathNavigator, INotificationService notificationService,
        IFileSystem fileSystem, IDialog dialog, IClipboard clipboard)
    {
        PathNavigator = pathNavigator;
        NotificationService = notificationService;
        FileSystem = fileSystem;
        Dialog = dialog;
        var directories = pathNavigator.Directories.Values()
            .Select(rooted => new DirectoryContentsViewModel(rooted, this)).ReplayLastActive();
        Directory = directories;
        SelectionContext = new SelectionContext(directories);

        SelectionContext.SelectionChanges.Bind(out var selectedItems).Subscribe().DisposeWith(disposable);
        
        Copy = directories.Select(d => ReactiveCommand.CreateFromTask(() =>
        {
            var dataObject = new DataObject();
            dataObject.Set("x-special/zafiro-copied-files", Serialize(selectedItems, d));
            return clipboard.SetDataObjectAsync(dataObject);
        }));
        
        Paste = ReactiveCommand.CreateFromTask(async () =>
        {
            var formatsAsync = await clipboard.GetFormatsAsync();
            var data = (byte[]?) await clipboard.GetDataAsync("x-special/zafiro-copied-files");
            var str = Encoding.UTF8.GetString(data!);
            var ob = JsonSerializer.Deserialize<ReadOnlyCollection<IDirectoryItem>>(str);
        });
    }

    private string Serialize(ReadOnlyObservableCollection<IDirectoryItem> selectedItems,
        DirectoryContentsViewModel directoryContentsViewModel)
    {
        var fileViewModels = selectedItems.OfType<FileViewModel>();
        var entries = fileViewModels.Select(f =>  "local:" + directoryContentsViewModel.Directory.Path.Combine(f.Name));
        var joinWithLines = entries.JoinWithLines();
        return JsonSerializer.Serialize(joinWithLines);
    }

    public ReactiveCommand<Unit,Unit> Paste { get; set; }

    public IObservable<ReactiveCommand<Unit, Unit>> Copy { get; set; }

    public SelectionContext SelectionContext { get; set; }

    public IObservable<DirectoryContentsViewModel> Directory { get; }

    [Reactive]
    public bool IsSelectionEnabled { get; set; }

    public void Dispose()
    {
        disposable.Dispose();
    }
}