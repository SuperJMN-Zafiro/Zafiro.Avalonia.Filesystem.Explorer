namespace Zafiro.Avalonia.FileExplorer.TransferManager.Items;

public class DesignTransferItem : ReactiveObject, ITransferItem
{
    public DesignTransferItem()
    {
        this.WhenAnyValue(x => x.SourceString).Select(s => (ZafiroPath)s).BindTo(this, x => x.Source);
        this.WhenAnyValue(x => x.DestinationString).Select(s => (ZafiroPath)s).BindTo(this, x => x.Destination);
        IsTransferring = true;
        DoTransfer = new StoppableCommand<Unit, Result>(_ => Observable.Return(Result.Success()), Maybe<IObservable<bool>>.None);
        EstimatedCompletion = Observable.Return(Maybe<TimeSpan>.From(TimeSpan.FromMinutes(1)));
    }

    public string Description => "Design-time";

    [Reactive]
    public ZafiroPath Source { get; set; }

    [Reactive]
    public string SourceString { get; set; }

    [Reactive]
    public string DestinationString { get; set; }

    [Reactive]
    public ZafiroPath Destination { get; set; }

    public IStoppableCommand<Unit, Result> DoTransfer { get; }
    public IObservable<LongProgress> Progress { get; set; }
    public bool IsTransferring { get; }
    public IObservable<string> Errors => Observable.Never<string>();
    public IObservable<Maybe<TimeSpan>> EstimatedCompletion { get; }
}