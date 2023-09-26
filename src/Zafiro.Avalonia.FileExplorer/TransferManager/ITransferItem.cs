using ReactiveUI;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public interface ITransferItem
{
    public TransferKey Key { get; }
    IReactiveCommand Transfer { get; }
}