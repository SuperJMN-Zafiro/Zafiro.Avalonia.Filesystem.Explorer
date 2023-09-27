using System;
using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.FileSystem.Actions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public class FileCopyViewModel : ReactiveObject, ITransferItem
{
    public FileCopyViewModel(CopyFileAction copyAction)
    {
        Source = copyAction.Source.Path;
        Destination = copyAction.Destination.Path;
        DoTransfer = StoppableCommandFactory.CreateFromTask(copyAction.Execute, Observable.Return(true));
        Progress = copyAction.Progress;
    }

    public string Source { get; }
    public string Destination { get; }
    public IStoppableCommand DoTransfer { get; }
    public IObservable<LongProgress> Progress { get; }
}