using Zafiro.Avalonia.FileExplorer.TransferManager.Items;

namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public interface ISelectionContext
{
    IObservable<bool> IsPasting { get; }
    ReactiveCommand<Unit, IList<ITransferItem>> Delete { get;  }
    ReactiveCommand<Unit, IList<ITransferItem>> Paste { get; }
    ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; }
    bool IsTouchFriendlySelectionEnabled { get; set; }
    ISelectionHandler SelectionHandler { get; }
}