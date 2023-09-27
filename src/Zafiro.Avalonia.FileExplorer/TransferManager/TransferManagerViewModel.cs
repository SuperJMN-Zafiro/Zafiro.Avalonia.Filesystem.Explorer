﻿using System;
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

        var ongoingTransfers = changeStream
            .AutoRefresh(x => x.IsTransferring)
            .Filter(x => x.IsTransferring);

        ongoingTransfers
            .Bind(out var ongoingTransfersCollection)
            .Subscribe();

        Transfers = transfers;
        OngoingTransfers = ongoingTransfersCollection;
        HasTransfers = changeStream.ToCollection().Select(x => x.Any()).StartWith(false);
        HasOngoingTransfers = ongoingTransfers.ToCollection().Select(x => x.Any()).StartWith(false);
    }

    public IObservable<bool> HasOngoingTransfers { get; }

    public void Add(ITransferItem item)
    {
        items.AddOrUpdate(item);
    }

    public ReadOnlyObservableCollection<ITransferItem> Transfers { get; }
    public IObservable<bool> HasTransfers { get; }
    public ReadOnlyObservableCollection<ITransferItem> OngoingTransfers { get; }
}