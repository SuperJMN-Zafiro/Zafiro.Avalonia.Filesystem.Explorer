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
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public class ToolBarViewModel
{
    private readonly BehaviorSubject<IZafiroDirectory> directory;

    public ToolBarViewModel(IObservable<IChangeSet<IEntry>> selection, IObservable<IZafiroDirectory> directories, IClipboard clipboard, ITransferManager transferManager, INotificationService notificationService)
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

        Copy
            .Do(_ => notificationService.Show("Copied to clipboard"))
            .Subscribe();

        var canPaste = clipboard.Contents.ToObservableChangeSet().ToCollection().Select(x => x.Any());

        Paste = ReactiveCommand.CreateFromTask(() => GenerateActions(clipboard.Contents), canPaste);
        Paste
            .Select(x => x.Successes())
            .Do(action =>
            {
                foreach (var action1 in action)
                {
                    if (action1 is CopyFileAction fc)
                    {
                        transferManager.Add(new FileCopyViewModel(fc));
                    }
                    if (action1 is CopyDirectoryAction dc)
                    {
                        transferManager.Add(new DirectoryCopyViewModel(dc));
                    }
                }
            }).Subscribe();
    }

    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Paste { get; set; }

    private async Task<IList<Result<IAction<LongProgress>>>> GenerateActions(IEnumerable<IClipboardItem> selectedItems)
    {
        var results = await selectedItems
            .ToObservable()
            .SelectMany(entry => GetAction(entry))
            .ToList();

        return results;
    }

    private Task<Result<IAction<LongProgress>>> GetAction(IClipboardItem entry)
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
        return directory.Value.FileSystem
            .GetDirectory(directory.Value.Path.Combine(directoryItem.Directory.Path.Name()))
            .Bind(async dst => await CopyDirectoryAction.Create(directoryItem.Directory, dst));
    }

    private Task<Result<CopyFileAction>> CreateFileTransfer(ClipboardFileItemViewModel fileItem)
    {
        return directory.Value.FileSystem
            .GetFile(directory.Value.Path.Combine(fileItem.Path.Name()))
            .Bind(async dst => await CopyFileAction.Create(fileItem.File, dst));
    }

    public ReactiveCommand<Unit, Unit> Copy { get; set; }
}