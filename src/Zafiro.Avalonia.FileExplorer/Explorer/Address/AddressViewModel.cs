﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.FileExplorer.ViewsModes.FolderContents;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.Mixins;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer.Address;

public class AddressViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<ZafiroPath> path;

    public AddressViewModel(IFileSystem fileSystem, DirectoryListing.Strategy strategy, INotificationService notificationService, IPendingActionsManager pendingActions, ITransferManager transferManager)
    {
        History = new History<ZafiroPath>(GetDefaultPath());

        GoToPath = ReactiveCommand.CreateFromTask(() => fileSystem.GetDirectory(RequestedPath!), this.WhenAnyValue(x => x.RequestedPath).NotNull());

        path = GoToPath.Successes()
            .Select(directory => directory.Path)
            .ToProperty(this, model => model.Path);

        GoToPath.Successes().Select(x => x.Path).BindTo(this, x => x.History.CurrentFolder);
       
        GoToPath.HandleErrorsWith(notificationService);

        GoBack = History.GoBack;
        IsNavigating = GoToPath.IsExecuting;

        PendingActions = pendingActions;
        TransferManager = transferManager;

        RequestedPath = "/";
    }

    public IPendingActionsManager PendingActions { get; }
    public ITransferManager TransferManager { get; }

    public ZafiroPath Path => path.Value;

    public ReactiveCommand<Unit, Unit> GoBack { get; set; }

    [Reactive] public string RequestedPath { get; set; }

    public History<ZafiroPath> History { get; }

    public ReactiveCommand<Unit, Result<IZafiroDirectory>> GoToPath { get; }

    public IObservable<bool> IsNavigating { get; }

    private static ZafiroPath GetDefaultPath()
    {
        return "/";
    }
}