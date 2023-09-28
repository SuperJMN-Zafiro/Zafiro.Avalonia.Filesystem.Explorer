using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Actions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public class DesignTransferItem : ReactiveObject, ITransferItem
{
    public DesignTransferItem()
    {
        this.WhenAnyValue(x => x.SourceString).Select(s => (ZafiroPath)s).BindTo(this, x => x.Source);
        this.WhenAnyValue(x => x.DestinationString).Select(s => (ZafiroPath)s).BindTo(this, x => x.Destination);
        IsTransferringObs = Observable.Return(true);
        IsTransferring = true;
        DoTransfer = new StoppableCommand<Unit, Result>(_ => Observable.Return(Result.Success()), Observable.Return(false));
    }

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
    public IObservable<bool> IsTransferringObs { get; }
    public bool IsTransferring { get; }
}