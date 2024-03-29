﻿using System.Reactive.Disposables;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Aggregation;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.FileExplorer.TransferManager.Items;
using Zafiro.FileSystem.Actions;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public class DeleteViewModel
{
    private readonly CompositeDisposable disposables = new();

    public DeleteViewModel(IObservable<IChangeSet<IEntry, string>> selectedItems, ITransferManager transferManager)
    {
        var canDelete = selectedItems.IsNotEmpty();
        selectedItems
            .Bind(out var selectedCollection)
            .Subscribe()
            .DisposeWith(disposables);

        Delete = ReactiveCommand.CreateFromObservable(() => DeleteActions(selectedCollection).Successes().Select(ToAction).ToList(), canDelete);
        Delete.Do(transferManager.Add)
            .Subscribe()
            .DisposeWith(disposables);
    }

    public ReactiveCommand<Unit, IList<ITransferItem>> Delete { get; }

    private static ITransferItem ToAction(IAction<LongProgress> action)
    {
        return action switch
        {
            DeleteFileAction df => new FileDeleteViewModel(df),
            DeleteDirectoryAction dd => new DirectoryDeleteViewModel(dd),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };
    }

    private static Task<Result<DeleteDirectoryAction>> DeleteDirectoryAction(DirectoryItemViewModel directoryItem) => Task.FromResult(Result.Success(new DeleteDirectoryAction(directoryItem.Directory)));

    private static Task<Result<DeleteFileAction>> DeleteFileAction(FileItemViewModel fileItem) => Task.FromResult(FileSystem.Actions.DeleteFileAction.Create(fileItem.File));

    private IObservable<Result<IAction<LongProgress>>> DeleteActions(IEnumerable<IEntry> selectedItems)
    {
        return selectedItems
            .ToObservable()
            .Select(entry => Observable.FromAsync(() => GetActionFromItem(entry)))
            .Concat();
    }

    private Task<Result<IAction<LongProgress>>> GetActionFromItem(IEntry entry)
    {
        var action = entry switch
        {
            FileItemViewModel file => DeleteFileAction(file).Map(action1 => (IAction<LongProgress>) action1),
            DirectoryItemViewModel dir => DeleteDirectoryAction(dir).Map(action1 => (IAction<LongProgress>) action1),
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };
        return action;
    }
}