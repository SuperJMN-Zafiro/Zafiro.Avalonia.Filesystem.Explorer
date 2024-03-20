using Zafiro.FileSystem.Actions;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.FileExplorer.TransferManager.Items;

public class DirectoryCopyViewModel : ReactiveObject, ITransferItem
{
    public DirectoryCopyViewModel(CopyDirectoryAction copyAction)
    {
        Source = copyAction.Source.Path;
        Destination = copyAction.Destination.Path;
        DoTransfer = StoppableCommand.CreateFromTask(copyAction.Execute, Maybe<IObservable<bool>>.None);
        Progress = copyAction.Progress;
        DoTransfer.IsExecuting.DelayItem(false, TimeSpan.FromSeconds(5)).BindTo(this, x => x.IsTransferring);
        Errors = DoTransfer.Start.Failures();
        EstimatedCompletion = Progress.Select(progress => progress.Value).EstimatedCompletion().Select(Maybe.From);
    }

    public IObservable<Maybe<TimeSpan>> EstimatedCompletion { get; }
    public string Description => $"Copy of {Source}";
    public ZafiroPath Source { get; }
    public ZafiroPath Destination { get; }
    public IStoppableCommand<Unit, Result> DoTransfer { get; }
    public IObservable<LongProgress> Progress { get; }

    [Reactive]
    public bool IsTransferring { get; private set; }

    public IObservable<string> Errors { get; }
}