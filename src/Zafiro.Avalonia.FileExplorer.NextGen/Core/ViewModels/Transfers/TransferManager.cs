using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

public class TransferManager : ITransferManager, IDisposable
{
    private readonly SourceList<ITransferItem> items = new();
    private readonly CompositeDisposable disposable = new();

    public TransferManager()
    {
        items.Connect()
            .Bind(out var transfers)
            .Subscribe()
            .DisposeWith(disposable);

        Transfers = transfers;
    }

    public ReadOnlyObservableCollection<ITransferItem> Transfers { get; }

    public void Add(params ITransferItem[] item)
    {
        items.AddRange(item);
    }

    public void Dispose()
    {
        items.Dispose();
        disposable.Dispose();
    }
}