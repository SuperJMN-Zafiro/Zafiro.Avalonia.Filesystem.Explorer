using Zafiro.FileSystem.Actions;

namespace Zafiro.Avalonia.FileExplorer.TransferManager.Items;

public class DirectoryDeleteViewModel : ReactiveObject, ITransferItem
{
    public DirectoryDeleteViewModel(DeleteDirectoryAction deleteDirectoryAction)
    {
        Source = deleteDirectoryAction.Directory.Path;
        DoTransfer = StoppableCommand.CreateFromTask(deleteDirectoryAction.Execute, Maybe<IObservable<bool>>.None);
        Progress = deleteDirectoryAction.Progress;
        DoTransfer.IsExecuting.DelayItem(false, TimeSpan.FromSeconds(5)).BindTo(this, x => x.IsTransferring);
        Errors = DoTransfer.Start.Failures();
        EstimatedCompletion = Observable.Return(Maybe<TimeSpan>.None);
    }

    public IObservable<Maybe<TimeSpan>> EstimatedCompletion { get; }

    public string Description => $"Delete {Source}";
    public ZafiroPath Source { get; }
    public ZafiroPath Destination { get; }
    public IStoppableCommand<Unit, Result> DoTransfer { get; }
    public IObservable<LongProgress> Progress { get; }
    public IObservable<bool> IsTransferringObs => DoTransfer.IsExecuting;

    [Reactive]
    public bool IsTransferring { get; private set; }

    public IObservable<string> Errors { get; }
}