using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer.Address;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.FileExplorer.ViewsModes.FolderContents;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.Mixins;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class ExplorerViewModel : ReactiveObject, IHaveResult<ZafiroPath>
{
    private readonly TaskCompletionSource<ZafiroPath> tck = new();

    public ExplorerViewModel(IFileSystem fileSystem, DirectoryListing.Strategy strategy, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager, INotificationService notificationService1)
    {
        Clipboard = clipboard;
        Address = new AddressViewModel(fileSystem, notificationService);
        TransferManager = transferManager;

        var detailsViewModels = Address.GoToPath.Successes()
            .Select(directory => new DetailsViewModel(directory, strategy, notificationService, clipboard, transferManager))
            .Publish()
            .RefCount();

        Details = detailsViewModels;
            
        var selectedItems = Details
            .Select(x => x.SelectedItems.ToObservableChangeSet())
            .Switch();

        ToolBar = new ToolBarViewModel(selectedItems, Address.GoToPath.Successes(), clipboard, transferManager, notificationService1);

        Details.Select(x => x.WhenAnyValue(x => x.SelectedItem)).Switch()
            .WhereNotNull()
            .OfType<FolderItemViewModel>()
            .Select(x => x.Path)
            .Merge(this.WhenAnyValue(x => x.Address.History.CurrentFolder))
            .Do(activatedFolder => Address.RequestedPath = activatedFolder.Path)
            .ToSignal()
            .InvokeCommand(Address.GoToPath);
    }

    public ITransferManager TransferManager { get; set; }

    public ToolBarViewModel ToolBar { get; }

    public AddressViewModel Address { get; }

    public IObservable<DetailsViewModel> Details { get; }
    public IClipboard Clipboard { get; }

    public Task<ZafiroPath> Result => tck.Task;

    public void SetResult(ZafiroPath result)
    {
        tck.SetResult(result);
    }
}