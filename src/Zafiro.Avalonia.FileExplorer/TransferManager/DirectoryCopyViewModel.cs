using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Actions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public class DirectoryCopyViewModel : ReactiveObject, ITransferItem
{
    public DirectoryCopyViewModel(CopyDirectoryAction copyAction)
    {
        Source = copyAction.Source.Path;
        Destination = copyAction.Destination.Path;
        DoTransfer = StoppableCommand.CreateFromTask(copyAction.Execute, Observable.Return(true));
        Progress = copyAction.Progress;
        DoTransfer.IsExecuting.BindTo(this, x => x.IsTransferring);
    }

    public ZafiroPath Source { get; }
    public ZafiroPath Destination { get; }
    public IStoppableCommand<Unit, Result> DoTransfer { get; }
    public IObservable<LongProgress> Progress { get; }
    public IObservable<bool> IsTransferringObs => DoTransfer.IsExecuting;

    [Reactive]
    public bool IsTransferring { get; private set; }
}