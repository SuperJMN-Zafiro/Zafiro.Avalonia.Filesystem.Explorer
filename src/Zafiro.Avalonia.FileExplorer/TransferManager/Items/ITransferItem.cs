using System.ComponentModel;

namespace Zafiro.Avalonia.FileExplorer.TransferManager.Items;

public interface ITransferItem : INotifyPropertyChanged
{
    public string Description { get; }
    ZafiroPath Source { get; }
    ZafiroPath Destination { get; }
    IStoppableCommand<Unit, Result> DoTransfer { get; }
    IObservable<LongProgress> Progress { get; }
    public bool IsTransferring { get; }
    public IObservable<string> Errors { get; }
    public IObservable<Maybe<TimeSpan>> EstimatedCompletion { get; }
}