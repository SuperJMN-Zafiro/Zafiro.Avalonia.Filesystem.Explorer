using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CSharpFunctionalExtensions;
using DynamicData.Aggregation;
using DynamicData.Binding;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public class ToolBarViewModel : IToolBar
{
    public ToolBarViewModel(ReadOnlyObservableCollection<IEntry> selection, IObservable<IZafiroDirectory> directories, IClipboard clipboard, ITransferManager transferManager, INotificationService notificationService)
    {
        var canCopy = selection
                .ToObservableChangeSet(x => x.Path)
                .IsNotEmpty();

        var directory = new BehaviorSubject<IZafiroDirectory?>(null);
        directories.Subscribe(directory);

        Copy = ReactiveCommand.Create(() =>
        {
            var clipboardItems = selection.Select(entry =>
            {
                return entry switch
                {
                    DirectoryItemViewModel di => (IClipboardItem)new ClipboardDirectoryItemViewModel(di.Directory),
                    FileItemViewModel fi => new ClipboardFileItemViewModel(fi.File),
                    _ => throw new ArgumentOutOfRangeException(nameof(entry))
                };
            }).ToList();

            clipboard.Add(clipboardItems);
            return clipboardItems;
        }, canCopy);

        Copy
            .Do(items => notificationService.Show(string.Join("\n", items.Count <= 3 ? items.Take(3).Select(x => x.Path.ToString()) : items.Take(3).Select(x => x.Path.ToString()).Concat(new[] { "..." })), "Copied to clipboard"))
            .Subscribe();

        var paste = new PasteViewModel(clipboard, directory, transferManager);
        Paste = paste.Paste;

        IsPasting = Paste.IsExecuting;

        var delete = new DeleteViewModel(selection, transferManager);
        Delete = delete.Delete;
    }

    public IObservable<bool> IsPasting { get; }

    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Delete { get; set; }

    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Paste { get; }

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; set; }
}