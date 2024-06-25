using Zafiro.Actions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

public interface ITransferItem
{
    public string Description { get; }
    IStoppableCommand<Unit, Result> Transfer { get; }

    IObservable<LongProgress> Progress { get; }
}