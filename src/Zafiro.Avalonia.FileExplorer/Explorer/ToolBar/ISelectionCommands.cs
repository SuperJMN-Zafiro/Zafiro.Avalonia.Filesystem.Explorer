using System;
using System.Collections.Generic;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Model;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public interface ISelectionCommands
{
    IObservable<bool> IsPasting { get; }
    ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Delete { get;  }
    ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Paste { get; }
    ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; }
}