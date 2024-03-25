using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.TransferManager.Items;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IToolBar
{
    ReactiveCommand<Unit, IList<ITransferItem>> Delete { get; set; }
    ReactiveCommand<Unit, IList<ITransferItem>> Paste { get; }
    ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; set; }
    ISelectionContext SelectionContext { get; }
}