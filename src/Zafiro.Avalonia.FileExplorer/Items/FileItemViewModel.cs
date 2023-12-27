using System;
using System.Collections.Generic;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class FileItemViewModel : ReactiveObject, IFile
{
    private readonly ISelectionCommands selectionCommands;
    public IZafiroFile File { get; }

    public FileItemViewModel(IZafiroFile file, IContentOpener fileOpener, ISelectionCommands selectionCommands, INotificationService notificationService)
    {
        this.selectionCommands = selectionCommands;
        File = file;
        Open = ReactiveCommand.CreateFromTask(() => fileOpener.Open(file.Contents, file.Path.Name()));
        Open.HandleErrorsWith(notificationService);
        IsBusy = Open.IsExecuting;
    }

    public IObservable<bool> IsBusy { get; }

    public ReactiveCommand<Unit, Result> Open { get; set; }

    public ZafiroPath Path => File.Path;

    public IObservable<byte> GetStream()
    {
        return File.Contents;
    }

    public string Name => Path.Name();

    public long Size { get; }

    [Reactive]
    public bool IsSelected { get; set; }

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy => selectionCommands.Copy;
    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Paste => selectionCommands.Paste;
}