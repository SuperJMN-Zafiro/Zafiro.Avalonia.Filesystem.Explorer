using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class FileSystemExplorer : ReactiveObject, IFileSystemExplorer
{
    private readonly TaskCompletionSource<ZafiroPath> tck = new();

    public FileSystemExplorer(IFileSystem fileSystem, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager)
    {
        Clipboard = clipboard;
        AddressViewModel = new Address.AddressViewModel(fileSystem, notificationService);
        TransferManager = transferManager;

        var detailsViewModels = AddressViewModel.LoadRequestedPath.Successes()
            .Select(directory => new DetailsViewModel(directory, new EverythingEntryFactory(AddressViewModel), notificationService, clipboard, transferManager))
            .Replay()
            .RefCount();

        Details = detailsViewModels;

        var source = detailsViewModels.Select(x => x.SelectedItems.ToObservableChangeSet()).Switch();
        source.Bind(out var collection).Subscribe();

        ToolBar = new ToolBarViewModel(collection, AddressViewModel.LoadRequestedPath.Successes(), clipboard, transferManager, notificationService);

        AddressViewModel.LoadRequestedPath.Execute().Take(1).Subscribe();
    }

    public ITransferManager TransferManager { get; }

    public ToolBarViewModel ToolBar { get; }

    public Address.AddressViewModel AddressViewModel { get; }

    public IObservable<DetailsViewModel> Details { get; }

    public IClipboard Clipboard { get; }

    public Task<ZafiroPath> Result => tck.Task;

    public void SetResult(ZafiroPath result)
    {
        tck.SetResult(result);
    }
}