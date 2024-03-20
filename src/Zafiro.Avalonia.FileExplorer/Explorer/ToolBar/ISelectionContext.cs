namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public interface ISelectionContext
{
    IObservable<bool> IsPasting { get; }
    ReactiveCommand<Unit, IList<IAction<LongProgress>>> Delete { get;  }
    ReactiveCommand<Unit, IAction<LongProgress>> Paste { get; }
    ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; }
    bool IsTouchFriendlySelectionEnabled { get; set; }
}