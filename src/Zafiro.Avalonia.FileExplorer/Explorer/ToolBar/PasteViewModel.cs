using System.Threading.Tasks;
using DynamicData.Aggregation;
using DynamicData.Binding;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.FileExplorer.TransferManager.Items;
using Zafiro.FileSystem.Actions;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public class PasteViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<IZafiroDirectory> currentDirectory;

    public PasteViewModel(IClipboard clipboard, IObservable<IZafiroDirectory> directories, ITransferManager transferManager)
    {
        currentDirectory = directories.ToProperty(this, x => x.CurrentDirectory);

        var canPaste = clipboard.Contents.ToObservableChangeSet().IsNotEmpty();

        Paste = ReactiveCommand.CreateFromTask(() => GenerateCopyActions(clipboard.Contents), canPaste);
        Paste
            .Select(x => x.Successes())
            .Do(actions =>
            {
                foreach (var act in actions)
                {
                    if (act is CopyFileAction fc)
                    {
                        transferManager.Add(new FileCopyViewModel(fc));
                    }
                    if (act is CopyDirectoryAction dc)
                    {
                        transferManager.Add(new DirectoryCopyViewModel(dc));
                    }
                }
            }).Subscribe();
    }

    public IZafiroDirectory CurrentDirectory => currentDirectory.Value;

    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Paste { get; }

    private async Task<IList<Result<IAction<LongProgress>>>> GenerateCopyActions(IEnumerable<IClipboardItem> selectedItems)
    {
        var results = await selectedItems
            .ToObservable()
            .SelectMany(GetCopyAction)
            .ToList();

        return results;
    }

    private Task<Result<IAction<LongProgress>>> GetCopyAction(IClipboardItem entry)
    {
        var action = entry switch
        {
            ClipboardFileItemViewModel folderItemViewModel => CreateFileTransfer(folderItemViewModel).Map(action1 => (IAction<LongProgress>)action1),
            ClipboardDirectoryItemViewModel fileItemViewModel => CreateDirectoryTransfer(fileItemViewModel).Map(action1 => (IAction<LongProgress>)action1),
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };
        return action;
    }
    private Task<Result<CopyDirectoryAction>> CreateDirectoryTransfer(ClipboardDirectoryItemViewModel directoryItem)
    {
        return Result.Success(CurrentDirectory.FileSystem
            .GetDirectory(CurrentDirectory.Path.Combine(directoryItem.Directory.Path.Name())))
            .Bind(async dst => await CopyDirectoryAction.Create(directoryItem.Directory, dst));
    }

    private Task<Result<CopyFileAction>> CreateFileTransfer(ClipboardFileItemViewModel fileItem)
    {
        return Result.Success(CurrentDirectory.FileSystem
            .GetFile(CurrentDirectory.Path.Combine(fileItem.Path.Name())))
            .Bind(async dst => await CopyFileAction.Create(fileItem.File, dst));
    }
}