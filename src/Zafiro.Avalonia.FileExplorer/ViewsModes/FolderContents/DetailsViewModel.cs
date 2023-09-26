using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;
using Zafiro.UI;
using static Zafiro.Avalonia.FileExplorer.Model.DirectoryListing;

namespace Zafiro.Avalonia.FileExplorer.ViewsModes.FolderContents;

public class DetailsViewModel : ReactiveObject
{
    private readonly IZafiroDirectory directory;

    public DetailsViewModel(IZafiroDirectory directory, Strategy strategy, INotificationService notificationService, IPendingActionsManager pendingActions, ITransferManager downloadManager)
    {
        this.directory = directory;
        SourceCache<IEntry, string> sourceCache = new(entry => entry.Path.Name());
        LoadChildren = ReactiveCommand.CreateFromTask(() => strategy(directory));

        LoadChildren.Successes().Do(entries => sourceCache.Edit(updater => updater.Load(entries))).Subscribe();
        LoadChildren.HandleErrorsWith(notificationService);

        var observable = sourceCache
            .Connect();

        observable
            .Sort(SortExpressionComparer<IEntry>.Descending(p => p is FolderItemViewModel)
                .ThenByAscending(p => p.Path.Name()))
            .Bind(out var collection)
            .Subscribe();

        observable
            .AutoRefresh(x => x.IsSelected)
            .Filter(x => x.IsSelected)
            .Bind(out var selectedItems)
            .Subscribe();

        HasSelection = observable
            .AutoRefresh(x => x.IsSelected)
            .Filter(x => x.IsSelected)
            .ToCollection()
            .Select(x => x.Any())
            .StartWith(false);

        SelectedItems = selectedItems;

        Children = collection;
        IsLoadingChildren = LoadChildren.IsExecuting.DelayItem(true, TimeSpan.FromSeconds(0.5));
        LoadChildren.Execute().Subscribe();
        Copy = ReactiveCommand.Create(() =>
        {
            var clipboardItems = selectedItems.Select(entry =>
            {
                return entry switch
                {
                    FolderItemViewModel di => (IClipboardItem)new ClipboardDirectoryItemViewModel(di.Directory),
                    FileItemViewModel fi => new ClipboardFileItemViewModel(fi.File),
                    _ => throw new ArgumentOutOfRangeException(nameof(entry))
                };
            });

            pendingActions.Copy(clipboardItems);
        });

        HasCopiedItems = pendingActions.Entries.ToObservableChangeSet().ToCollection().Select(x => x.Any());

        Paste = ReactiveCommand.CreateFromTask(() => GenerateActions(SelectedItems), HasCopiedItems);
        Paste
            .Successes()
            .Do(action =>
            {
                if (action is CopyFileAction copy)
                {
                    downloadManager.Add(new FileCopyViewModel(copy));
                }
            })
            .Subscribe();
    }

    private async Task<Result<IAction<LongProgress>>> GenerateActions(IEnumerable<IEntry> selectedItems)
    {
        return await selectedItems
            .ToObservable()
            .SelectMany(entry => GetAction(entry));
    }

    private async Task<Result<IAction<LongProgress>>> GetAction(IEntry entry)
    {
        var action = entry switch
        {
            FolderItemViewModel folderItemViewModel => await CreateFileTransfer(folderItemViewModel).Map(action1 => (IAction<LongProgress>)action1),
            FileItemViewModel fileItemViewModel => await CreateDirectoryTransfer(fileItemViewModel).Map(action1 => (IAction<LongProgress>)action1),
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };
        return action;
    }

    private Task<Result<CopyFileAction>> CreateDirectoryTransfer(FileItemViewModel fileItemViewModel)
    {
        return directory.FileSystem.GetFile(directory.Path.Combine(fileItemViewModel.Name)).Bind(async dst => await CopyFileAction.Create(fileItemViewModel.File, dst));
    }

    private async Task<Result<IAction<LongProgress>>> CreateFileTransfer(FolderItemViewModel folderItemViewModel)
    {
        return await CopyDirectoryAction.Create(folderItemViewModel.Directory, directory).Map(action1 => (IAction<LongProgress>)action1);
    }


    public ReactiveCommand<Unit, Result<IAction<LongProgress>>> Paste { get; set; }

    public IObservable<bool> HasSelection { get; }

    public ReadOnlyObservableCollection<IEntry> SelectedItems { get; }

    public IObservable<bool> IsLoadingChildren { get; }

    public ReactiveCommand<Unit, Result<IEnumerable<IEntry>>> LoadChildren { get; }

    public ZafiroPath Path => directory.Path;

    public ReadOnlyObservableCollection<IEntry> Children { get; }

    [Reactive] public IEntry SelectedItem { get; set; }

    public string Name => Path.Name();
    public ICommand Copy { get; }
    public IObservable<bool> HasCopiedItems { get; }
}



public interface ICopyAction : IAction<LongProgress>
{
    public ZafiroPath Source { get; set; }
    public ZafiroPath Destination { get; set; }
}


