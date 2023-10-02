﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.Mixins;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.ViewsModes.FolderContents;

public class FolderContentsViewModel : ReactiveObject, IHaveResult<ZafiroPath>
{
    private readonly ObservableAsPropertyHelper<DetailsViewModel> details;
    private readonly ObservableAsPropertyHelper<ZafiroPath> path;
    private TaskCompletionSource<ZafiroPath> tck = new();

    public FolderContentsViewModel(IFileSystem fileSystem, IDirectoriesEntryFactory strategy, INotificationService notificationService, IClipboard pendingActions, ITransferManager transferManager)
    {
        History = new History<ZafiroPath>(GetDefaultPath());

        GoToPath = ReactiveCommand.CreateFromTask(() => fileSystem.GetDirectory(RequestedPath!), this.WhenAnyValue(x => x.RequestedPath).NotNull());

        details = GoToPath.Successes()
            .Select(directory => new DetailsViewModel(directory, strategy, notificationService, pendingActions, transferManager))
            .ToProperty(this, model => model.Details);

        path = GoToPath.Successes()
            .Select(directory => directory.Path)
            .ToProperty(this, model => model.Path);

        GoToPath.Successes().Select(x => x.Path).BindTo(this, x => x.History.CurrentFolder);
        this.WhenAnyValue(x => x.Details.SelectedItem).OfType<DirectoryItemViewModel>()
            .Select(x => x.Path)
            .Merge(this.WhenAnyValue(x => x.History.CurrentFolder))
            .Do(activatedFolder => RequestedPath = activatedFolder.Path)
            .ToSignal()
            .InvokeCommand(GoToPath);

        GoToPath.HandleErrorsWith(notificationService);

        GoBack = History.GoBack;
        IsNavigating = GoToPath.IsExecuting.CombineLatest(this.WhenAnyObservable(model => model.Details.IsLoadingChildren), (b, b1) => b || b1);

        TransferManager = transferManager;

        RequestedPath = "/";
    }

    public ITransferManager TransferManager { get; }

    public ZafiroPath Path => path.Value;

    public ReactiveCommand<Unit, Unit> GoBack { get; set; }

    [Reactive] public string RequestedPath { get; set; }

    public History<ZafiroPath> History { get; }

    public DetailsViewModel Details => details.Value;

    public ReactiveCommand<Unit, Result<IZafiroDirectory>> GoToPath { get; }

    public IObservable<bool> IsNavigating { get; }

    private static ZafiroPath GetDefaultPath()
    {
        return "/";
    }

    public Task<ZafiroPath> Result => tck.Task;

    public void SetResult(ZafiroPath result)
    {
        tck.SetResult(result);
    }
}