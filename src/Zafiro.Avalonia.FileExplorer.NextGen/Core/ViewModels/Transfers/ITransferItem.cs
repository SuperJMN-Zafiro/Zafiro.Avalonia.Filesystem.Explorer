using Zafiro.Actions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

public interface ITransferItem
{
    public string Description { get; }
    IStoppableCommand<Unit, Result> StartCommand { get; }

    IObservable<LongProgress> Progress { get; }
}