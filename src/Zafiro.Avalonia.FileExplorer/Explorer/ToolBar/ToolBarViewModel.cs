using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public class ToolBarViewModel
{
    private readonly BehaviorSubject<IZafiroDirectory> directory;

    public ToolBarViewModel(IObservable<IChangeSet<IEntry>> selection, IObservable<IZafiroDirectory> directories, IClipboard clipboard, ITransferManager transferManager)
    {
        var canCopy = selection
            .ToCollection()
            .Select(x => x.Any());

        selection
            .Bind(out var items)
            .Subscribe();

        directory = new BehaviorSubject<IZafiroDirectory>(null);
        directories.Subscribe(directory);
        
        Copy = ReactiveCommand.Create(() =>
        {
            var clipboardItems = items.Select(entry =>
            {
                return entry switch
                {
                    FolderItemViewModel di => (IClipboardItem)new ClipboardDirectoryItemViewModel(di.Directory),
                    FileItemViewModel fi => new ClipboardFileItemViewModel(fi.File),
                    _ => throw new ArgumentOutOfRangeException(nameof(entry))
                };
            });

            clipboard.Add(clipboardItems);
        }, canCopy);

        var canPaste = clipboard.Entries.ToObservableChangeSet().ToCollection().Select(x => x.Any());

        Paste = ReactiveCommand.CreateFromTask(() => GenerateActions(clipboard.Entries), canPaste);
        Paste
            .Successes()
            .Do(action =>
            {
                if (action is CopyFileAction copy)
                {
                    transferManager.Add(new FileCopyViewModel(copy));
                }
            });
    }

    public ReactiveCommand<Unit, Result<IAction<LongProgress>>> Paste { get; set; }

    private async Task<Result<IAction<LongProgress>>> GenerateActions(IEnumerable<IClipboardItem> selectedItems)
    {
        return await selectedItems
            .ToObservable()
            .SelectMany(entry => GetAction(entry));
    }

    private async Task<Result<IAction<LongProgress>>> GetAction(IClipboardItem entry)
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
        return directory.Value.FileSystem.GetFile(directory.Value.Path.Combine(fileItemViewModel.Name)).Bind(async dst => await CopyFileAction.Create(fileItemViewModel.File, dst));
    }

    private async Task<Result<IAction<LongProgress>>> CreateFileTransfer(FolderItemViewModel folderItemViewModel)
    {
        return await CopyDirectoryAction.Create(folderItemViewModel.Directory, directory.Value).Map(action1 => (IAction<LongProgress>)action1);
    }

    public ReactiveCommand<Unit, Unit> Copy { get; set; }
}