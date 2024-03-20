namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public interface ISelectionContext
{
    IObservable<bool> IsPasting { get; }
    ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Delete { get;  }
    ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Paste { get; }
    ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; }
    bool IsTouchFriendlySelectionEnabled { get; set; }
}