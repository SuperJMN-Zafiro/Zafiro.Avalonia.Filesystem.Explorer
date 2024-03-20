using System.Linq;
using DynamicData;
using DynamicData.Binding;
using Zafiro.Avalonia.FileExplorer.TransferManager.Items;

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

        var ongoingTransfers = changeStream
            .AutoRefresh(x => x.IsTransferring)
            .Filter(x => x.IsTransferring);

        ongoingTransfers
            .Bind(out var ongoingTransfersCollection)
            .Subscribe();

        Transfers = transfers;
        OngoingTransfers = ongoingTransfersCollection;
        HasTransfers = changeStream.ToCollection().StartWithEmpty().Select(x => x.Any());
        HasOngoingTransfers = ongoingTransfers.StartWithEmpty().ToCollection().Select(x => x.Any());

        Transfers
            .ToObservableChangeSet()
            .OnItemAdded(r =>
            {
                if (AutoStartOnAdd)
                {
                    r.DoTransfer.Start.Execute().Take(1).Subscribe();
                }
            })
            .Subscribe();
    }

    public bool AutoStartOnAdd { get; set; }

    public IObservable<bool> HasOngoingTransfers { get; }

    public void Add(ITransferItem item)
    {
        items.AddOrUpdate(item);
    }

    public ReadOnlyObservableCollection<ITransferItem> Transfers { get; }
    public IObservable<bool> HasTransfers { get; }
    public ReadOnlyObservableCollection<ITransferItem> OngoingTransfers { get; }
}