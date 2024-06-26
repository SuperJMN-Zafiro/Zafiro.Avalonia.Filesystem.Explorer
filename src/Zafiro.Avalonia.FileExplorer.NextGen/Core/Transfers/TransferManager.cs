using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Aggregation;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.Transfers;

public class TransferManager : ITransferManager, IDisposable
{
    private readonly SourceCache<ITransferItem, object> items = new(item => item);
    private readonly CompositeDisposable disposable = new();

    public TransferManager()
    {
        var itemChanges = items.Connect();

        itemChanges
            .Bind(out var transfers)
            .Subscribe()
            .DisposeWith(disposable);

        Transfers = transfers;

        // var current = items.Connect().TransformOnObservable(item => item.Progress.Select(x => x.Current)).Sum(l => l);
        // var total = items.Connect().TransformOnObservable(item => item.Progress.Select(x => x.Total)).Sum(l => l);
        // Progress = total.Where(l => l > 0).WithLatestFrom(current, (t, c) => c / t);
        Progress = itemChanges
            .FilterOnObservable(x => x.Transfer.IsExecuting)
            .TransformOnObservable(x => x.Progress.Select(y => y.Value)).Avg(d => d).Sample(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler);

        IsTransferring = items.Connect(suppressEmptyChangeSets: false)
            .FilterOnObservable(x => x.Transfer.IsExecuting)
            .Count()
            .Select(i => i > 0);

        itemChanges.FilterOnObservable(item => item.Transfer.IsExecuting.Not().Skip(1).Delay(TimeSpan.FromSeconds(5), RxApp.MainThreadScheduler))
            .OnItemAdded(item => items.Remove(item))
            .Subscribe()
            .DisposeWith(disposable);
    }

    public IObservable<bool> IsTransferring { get; }

    public IObservable<double> Progress { get; }

    public ReadOnlyObservableCollection<ITransferItem> Transfers { get; }

    public void Add(params ITransferItem[] item)
    {
        items.AddOrUpdate(item);
    }

    public void Dispose()
    {
        items.Dispose();
        disposable.Dispose();
    }
}