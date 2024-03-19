using System;
using System.Collections.Generic;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Model;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public class ToolBarViewModel : IToolBar
{
    private readonly ISelectionCommands selectionCommands;

    public ToolBarViewModel(ISelectionCommands selectionCommands)
    {
        this.selectionCommands = selectionCommands;
        Delete = selectionCommands.Delete;
        Copy = selectionCommands.Copy;
        Paste = selectionCommands.Paste;
        IsPasting = selectionCommands.IsPasting;
    }

    public IObservable<bool> IsPasting { get; }

    public bool IsSelectionEnabled
    {
        get => selectionCommands.IsTouchFriendlySelectionEnabled;
        set => selectionCommands.IsTouchFriendlySelectionEnabled = value;
    }

    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Delete { get; set; }

    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Paste { get; }

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; set; }
}