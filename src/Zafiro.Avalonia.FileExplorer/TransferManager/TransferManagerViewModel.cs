using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public class TransferManagerViewModel : ITransferManager
{
    private readonly SourceCache<ITransferItem, TransferKey> items = new(x => new TransferKey(x.Source, x.Destination));

    public TransferManagerViewModel()
    {
        var changeStream = items
            .Connect();

        changeStream
            .Bind(out var transfers)
            .Subscribe();

        Transfers = transfers;
        HasTransfers = changeStream.ToCollection().Select(x => x.Any());
    }

    public void Add(ITransferItem item)
    {
        items.AddOrUpdate(item);
    }

    public ReadOnlyObservableCollection<ITransferItem> Transfers { get; }
    public IObservable<bool> HasTransfers { get; }
}