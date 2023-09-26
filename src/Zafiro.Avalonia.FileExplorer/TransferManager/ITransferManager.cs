using Zafiro.Avalonia.FileExplorer.ViewsModes.FolderContents;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public interface ITransferManager
{
    void Add(ITransferItem item);
}