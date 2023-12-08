using System;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer.Address;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class FileSystemExplorer : ReactiveObject, IFileSystemExplorer
{
    public FileSystemExplorer(IFileSystemRoot fileSystem, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager)
    {
        Clipboard = clipboard;
        Address = new AddressViewModel(fileSystem, notificationService);
        TransferManager = transferManager;

        var detailsViewModels = Address.LoadRequestedPath.Successes()
            .Select(directory => new DetailsViewModel(directory, new EverythingEntryFactory(Address), notificationService, clipboard, transferManager))
            .Replay()
            .RefCount();

        Details = detailsViewModels;

        var source = detailsViewModels.Select(x => x.SelectedItems.ToObservableChangeSet()).Switch();
        source.Bind(out var collection).Subscribe();

        ToolBar = new ToolBarViewModel(collection, Address.LoadRequestedPath.Successes(), clipboard, transferManager, notificationService);
        InitialPath.Or(ZafiroPath.Empty).Execute(GoTo);
    }

    public Maybe<ZafiroPath> InitialPath { get; init; }

    public IObservable<Maybe<IZafiroDirectory>> CurrentDirectory => Address.CurrentDirectory;

    public ITransferManager TransferManager { get; }

    public IToolBar ToolBar { get; }

    public IAddress Address { get; }

    public IObservable<DetailsViewModel> Details { get; }

    public IClipboard Clipboard { get; }

    public void GoTo(ZafiroPath path)
    {
        Address.SetAndLoad(path);
    }
}