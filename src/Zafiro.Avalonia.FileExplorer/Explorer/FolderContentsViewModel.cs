using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Explorer.Address;
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
    private readonly ObservableAsPropertyHelper<DetailsViewModel> details;
    private readonly TaskCompletionSource<ZafiroPath> tck = new();

    public ExplorerViewModel(IFileSystem fileSystem, DirectoryListing.Strategy strategy, INotificationService notificationService, IPendingActionsManager pendingActions, ITransferManager transferManager)
    {
        Address = new AddressViewModel(fileSystem, strategy, notificationService, pendingActions, transferManager);
        
        details = Address.GoToPath.Successes()
            .Select(directory => new DetailsViewModel(directory, strategy, notificationService, pendingActions, transferManager))
            .ToProperty(this, model => model.Details);

        this.WhenAnyValue(x => x.Details.SelectedItem).OfType<FolderItemViewModel>()
            .Select(x => x.Path)
            .Merge(this.WhenAnyValue(x => x.Address.History.CurrentFolder))
            .Do(activatedFolder => Address.RequestedPath = activatedFolder.Path)
            .ToSignal()
            .InvokeCommand(Address.GoToPath);

        // TODO: Enable if needed
        //IsNavigating = Address.GoToPath.IsExecuting.CombineLatest(this.WhenAnyObservable(model => model.Details.IsLoadingChildren), (b, b1) => b || b1);
    }

    public AddressViewModel Address { get; set; }

    public DetailsViewModel Details => details.Value;

    // TODO: Enable if needed
    //public IObservable<bool> IsNavigating { get; }

    public Task<ZafiroPath> Result => tck.Task;

    public void SetResult(ZafiroPath result)
    {
        tck.SetResult(result);
    }
}