using System;
using System.Threading.Tasks;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer;

public interface IFileSystemExplorer : IResult<ZafiroPath>
{
    ITransferManager TransferManager { get; }
    ToolBarViewModel ToolBar { get; }
    Address.AddressViewModel AddressViewModel { get; }
    IObservable<DetailsViewModel> Details { get; }
    IClipboard Clipboard { get; }
    Task<ZafiroPath> Result { get; }
}