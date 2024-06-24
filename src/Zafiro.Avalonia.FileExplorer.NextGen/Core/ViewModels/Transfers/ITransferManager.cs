using System.Collections.ObjectModel;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

public interface ITransferManager
{
    public void Add(params ITransferItem[] item);
    ReadOnlyObservableCollection<ITransferItem> Transfers { get; }
    IObservable<double> Progress { get; }
    IObservable<bool> IsTransferring { get; }
}