using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using DynamicData;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

public class TransferManager : ITransferManager, IDisposable
{
    private SourceCache<ITransferItem, string> cache = new(item => item.Key);
    private readonly CompositeDisposable disposable = new();

    public TransferManager()
    {
        cache.Connect()
            .Bind(out var transfers)
            .Subscribe()
            .DisposeWith(disposable);
        
        Transfers = transfers;
    }

    public ReadOnlyObservableCollection<ITransferItem> Transfers { get; }

    public void Add(ITransferItem item)
    {
        cache.AddOrUpdate(item);
    }

    public void Dispose()
    {
        cache.Dispose();
        disposable.Dispose();
    }
}