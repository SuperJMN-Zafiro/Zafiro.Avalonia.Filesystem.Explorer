using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.FileExplorer.TransferManager.Items;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public class DeleteViewModel
{
    public DeleteViewModel(ReadOnlyObservableCollection<IEntry> selectedItems, ITransferManager transferManager)
    {
        var canDelete = selectedItems.ToObservableChangeSet().ToCollection().Select(x => x.Any());

        Delete = ReactiveCommand.CreateFromTask(() => GenerateCopyActions(selectedItems), canDelete);
        Delete
            .Select(x => x.Successes())
            .Do(actions =>
            {
                foreach (var act in actions)
                {
                    if (act is DeleteFileAction df)
                    {
                        transferManager.Add(new FileDeleteViewModel(df));
                    }
                    if (act is DeleteDirectoryAction dd)
                    {
                        transferManager.Add(new DirectoryDeleteViewModel(dd));
                    }
                }
            }).Subscribe();
    }

    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Delete { get; }

    private async Task<IList<Result<IAction<LongProgress>>>> GenerateCopyActions(IEnumerable<IEntry> selectedItems)
    {
        var results = await selectedItems
            .ToObservable()
            .SelectMany(GetActionFromItem)
            .ToList();

        return results;
    }

    private Task<Result<IAction<LongProgress>>> GetActionFromItem(IEntry entry)
    {
        var action = entry switch
        {
            FileItemViewModel folderItemViewModel => CreateFileTransfer(folderItemViewModel).Map(action1 => (IAction<LongProgress>)action1),
            DirectoryItemViewModel fileItemViewModel => CreateDirectoryTransfer(fileItemViewModel).Map(action1 => (IAction<LongProgress>)action1),
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };
        return action;
    }
    private Task<Result<DeleteDirectoryAction>> CreateDirectoryTransfer(DirectoryItemViewModel directoryItem)
    {
        return DeleteDirectoryAction.Create(directoryItem.Directory);
    }

    private Task<Result<DeleteFileAction>> CreateFileTransfer(FileItemViewModel fileItem)
    {
        return Task.FromResult(DeleteFileAction.Create(fileItem.File));
    }
}

