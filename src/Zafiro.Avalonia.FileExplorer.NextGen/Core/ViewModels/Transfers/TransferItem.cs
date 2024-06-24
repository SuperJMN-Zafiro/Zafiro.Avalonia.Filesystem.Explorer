using System.Reactive.Linq;
using Zafiro.Actions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

internal class TransferItem : ReactiveObject, ITransferItem
{
    public TransferItem(string description, IAction<LongProgress> action)
    {
        Description = description;
        Action = action;
        StartCommand = StoppableCommand.CreateFromTask(ct => Action.Execute(ct), Maybe.From(Observable.Return(true)));
        Progress = action.Progress;
    }

    public IObservable<LongProgress> Progress { get; }

    public IStoppableCommand<Unit,Result> StartCommand { get; set; }

    public string Description { get; }

    public IAction<LongProgress> Action { get; }
}