using Zafiro.Actions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

public interface ITransferItem
{
    string Key { get; }

    IStoppableCommand<Unit, Result> StartCommand { get; set; }

    IObservable<LongProgress> Progress { get; }

    IObservable<bool> IsTransferring { get; }
    // Task Start(CancellationToken cancellationToken);
}