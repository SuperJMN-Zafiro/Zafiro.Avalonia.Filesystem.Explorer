using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer.Address;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.Mixins;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public class FileSystemExplorer : ReactiveObject, IFileSystemExplorer, IDisposable, ISelectionCommands
{
    private readonly ObservableAsPropertyHelper<DirectoryContentsViewModel> details;
    private readonly CompositeDisposable disposable = new();
    private readonly SelectionCommands selectionCommands;

    public FileSystemExplorer(IFileSystemRoot fileSystem, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager, IContentOpener opener)
    {
        Clipboard = clipboard;
        PathNavigator = new PathNavigatorViewModel(fileSystem, notificationService);
        TransferManager = transferManager;

        var detailsViewModels = PathNavigator.LoadRequestedPath.Successes()
            .Select(directory => new DirectoryContentsViewModel(directory, new EverythingEntryFactory(PathNavigator, opener, notificationService, this), PathNavigator, notificationService, opener, this))
            .ReplayLastActive();

        details = detailsViewModels.ToProperty(this, explorer => explorer.Details)
            .DisposeWith(disposable);

        SerialDisposable serialDisposable = new();
        this.WhenAnyValue(x => x.Details)
            .Do(d => serialDisposable.Disposable = d)
            .Subscribe()
            .DisposeWith(disposable);

        this.WhenAnyValue(x => x.Details.SelectedItems)
            .Bind(out var selectedItems)
            .DisposeWith(disposable);

        selectionCommands = new SelectionCommands(selectedItems, PathNavigator.LoadRequestedPath.Successes(), clipboard, transferManager, notificationService);
        ToolBar = new ToolBarViewModel(this);

        InitialPath.Or(ZafiroPath.Empty).Execute(GoTo);
    }

    public Maybe<ZafiroPath> InitialPath { get; init; }

    public void Dispose()
    {
        details.Dispose();
    }

    public IObservable<Maybe<IZafiroDirectory>> CurrentDirectory => PathNavigator.CurrentDirectory;

    public ITransferManager TransferManager { get; }

    public IToolBar ToolBar { get; }

    public IPathNavigator PathNavigator { get; }

    public DirectoryContentsViewModel Details => details.Value;

    public IClipboard Clipboard { get; }

    public void GoTo(ZafiroPath path)
    {
        PathNavigator.SetAndLoad(path);
    }

    public IObservable<bool> IsPasting => ((ISelectionCommands) selectionCommands).IsPasting;

    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Delete => selectionCommands.Delete;

    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Paste => selectionCommands.Paste;

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy => selectionCommands.Copy;
}