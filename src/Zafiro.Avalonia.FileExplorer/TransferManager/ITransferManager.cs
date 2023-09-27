using System;
using System.Collections.ObjectModel;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public interface ITransferManager
{
    void Add(ITransferItem item);
    ReadOnlyObservableCollection<ITransferItem> Transfers { get; }
    public IObservable<bool> HasTransfers { get; }
}