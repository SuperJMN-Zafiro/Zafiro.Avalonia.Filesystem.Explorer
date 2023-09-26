using System;
using System.Collections.ObjectModel;
using DynamicData;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public class TransferManagerViewModel : ITransferManager
{
    private SourceCache<ITransferItem, TransferKey> items = new(x => x.Key);

    public TransferManagerViewModel()
    {
        items
            .Connect()
            .Bind(out var transfers)
            .Subscribe();

        Transfers = transfers;
    }

    public void Add(ITransferItem item)
    {
        items.AddOrUpdate(item);
    }

    public ReadOnlyObservableCollection<ITransferItem> Transfers { get; }
}