using System;
using System.Threading.Tasks;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public interface IFileSystemExplorer : IHaveResult<ZafiroPath>
{
    ITransferManager TransferManager { get; }
    ToolBarViewModel ToolBar { get; }
    Address.AddressViewModel AddressViewModel { get; }
    IObservable<DetailsViewModel> Details { get; }
    IClipboard Clipboard { get; }
    Task<ZafiroPath> Result { get; }
}