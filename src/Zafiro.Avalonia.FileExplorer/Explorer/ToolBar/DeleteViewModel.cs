using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.FileExplorer.TransferManager.Items;
using Zafiro.FileSystem.Actions;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public class DeleteViewModel
{
    public DeleteViewModel(ReadOnlyObservableCollection<IEntry> selectedItems, ITransferManager transferManager)
    {
        var canDelete = selectedItems.ToObservableChangeSet().ToCollection().Select(x => x.Any());

        Delete = ReactiveCommand.CreateFromTask(() => DeleteItems(selectedItems), canDelete);
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

    private async Task<IList<Result<IAction<LongProgress>>>> DeleteItems(IEnumerable<IEntry> selectedItems)
    {
        var results = await selectedItems
            .ToObservable()
            .Select(entry => Observable.FromAsync(() => GetActionFromItem(entry)))
            .Merge(1)
            .ToList();

        return results;
    }

    private Task<Result<IAction<LongProgress>>> GetActionFromItem(IEntry entry)
    {
        var action = entry switch
        {
            FileItemViewModel file => DeleteFileAction(file).Map(action1 => (IAction<LongProgress>)action1),
            DirectoryItemViewModel dir => DeleteDirectoryAction(dir).Map(action1 => (IAction<LongProgress>)action1),
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };
        return action;
    }
    private Task<Result<DeleteDirectoryAction>> DeleteDirectoryAction(DirectoryItemViewModel directoryItem)
    {
        return Task.FromResult(Result.Success(new DeleteDirectoryAction(directoryItem.Directory)));
    }

    private Task<Result<DeleteFileAction>> DeleteFileAction(FileItemViewModel fileItem)
    {
        return Task.FromResult(FileSystem.Actions.DeleteFileAction.Create(fileItem.File));
    }
}

