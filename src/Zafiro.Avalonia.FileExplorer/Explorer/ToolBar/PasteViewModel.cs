using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.Avalonia.FileExplorer.TransferManager.Items;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public class PasteViewModel
{
    private readonly BehaviorSubject<IZafiroDirectory> directory;

    public PasteViewModel(IClipboard clipboard, BehaviorSubject<IZafiroDirectory> directory, ITransferManager transferManager)
    {
        this.directory = directory;

        var canPaste = clipboard.Contents.ToObservableChangeSet().ToCollection().Select(x => x.Any());

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
        return Result.Success(directory.Value.FileSystem
            .GetDirectory(directory.Value.Path.Combine(directoryItem.Directory.Path.Name())))
            .Bind(async dst => await CopyDirectoryAction.Create(directoryItem.Directory, dst));
    }

    private Task<Result<CopyFileAction>> CreateFileTransfer(ClipboardFileItemViewModel fileItem)
    {
        return Result.Success(directory.Value.FileSystem
            .GetFile(directory.Value.Path.Combine(fileItem.Path.Name())))
            .Bind(async dst => await CopyFileAction.Create(fileItem.File, dst));
    }
}