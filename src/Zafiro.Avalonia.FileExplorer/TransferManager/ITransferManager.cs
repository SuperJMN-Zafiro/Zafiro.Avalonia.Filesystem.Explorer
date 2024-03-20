using Zafiro.Avalonia.FileExplorer.TransferManager.Items;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public interface ITransferManager
{
    void Add(ITransferItem item);
    ReadOnlyObservableCollection<ITransferItem> Transfers { get; }
    public IObservable<bool> HasTransfers { get; }
    public ReadOnlyObservableCollection<ITransferItem> OngoingTransfers { get; }
    IObservable<bool> HasOngoingTransfers { get; }
}

public static class TransferManagerExtensions
{
    public static void Add(this ITransferManager transferManager, IEnumerable<ITransferItem> items)
    {
        foreach (var item in items)
        {
            transferManager.Add(item);
        }
    }
}