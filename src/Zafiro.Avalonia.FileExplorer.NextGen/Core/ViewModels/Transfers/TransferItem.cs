using System.Reactive.Concurrency;
using Zafiro.Actions;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

internal class TransferItem : ReactiveObject, ITransferItem
{
    public TransferItem(string description, IAction<LongProgress> action)
    {
        Description = description;
        Action = action;
        Start = ReactiveCommand.CreateFromTask(ct => Action.Execute(ct, new NewThreadScheduler(start => new Thread(start)
        {
             Priority = ThreadPriority.Normal,
             IsBackground = true,
        })));
        Progress = action.Progress;
    }

    public IObservable<LongProgress> Progress { get; }

    public ReactiveCommand<Unit, Result> Start { get; set; }

    public string Description { get; }

    public IAction<LongProgress> Action { get; }
}