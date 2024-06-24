using Zafiro.Actions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

public interface ITransferItem
{
    public string Description { get; }
    ReactiveCommand<Unit, Result> Start { get; }

    IObservable<LongProgress> Progress { get; }
}