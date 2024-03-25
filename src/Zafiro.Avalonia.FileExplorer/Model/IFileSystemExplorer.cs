using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.TransferManager;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IFileSystemExplorer
{
    ITransferManager TransferManager { get; }
    IToolBar ToolBar { get; }
    IPathNavigator PathNavigator { get; }
    DirectoryContentsViewModel Details { get; }
    IClipboard Clipboard { get; }
    IObservable<Maybe<IZafiroDirectory>> CurrentDirectory { get; }
    ISelectionContext SelectionContext { get; }
    void GoTo(ZafiroPath path);
}