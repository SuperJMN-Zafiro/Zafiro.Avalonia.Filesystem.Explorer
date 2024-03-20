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

        Paste = ReactiveCommand.CreateFromObservable(() =>
        {
            return CopyActions(clipboard.Contents)
                .Successes()
                .Do(action => transferManager.Add(ToAction(action)));
        }, canPaste);
    }

    private static ITransferItem ToAction(IAction<LongProgress> action)
    {
        return action switch
        {
            CopyFileAction fc => new FileCopyViewModel(fc),
            CopyDirectoryAction dc => new DirectoryCopyViewModel(dc),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };
    }

    public IZafiroDirectory CurrentDirectory => currentDirectory.Value;

    public ReactiveCommand<Unit, IAction<LongProgress>> Paste { get; }

    private IObservable<Result<IAction<LongProgress>>> CopyActions(IEnumerable<IClipboardItem> selectedItems)
    {
        return selectedItems
            .ToObservable()
            .SelectMany(GetCopyAction);
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