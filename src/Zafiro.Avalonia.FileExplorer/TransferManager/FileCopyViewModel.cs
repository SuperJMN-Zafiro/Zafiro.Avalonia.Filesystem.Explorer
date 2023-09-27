using System;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public class FileCopyViewModel : ReactiveObject, ITransferItem
{
    public FileCopyViewModel(CopyFileAction copyAction)
    {
        Source = copyAction.Source.Path;
        Destination = copyAction.Destination.Path;
        Transfer = ReactiveCommand.CreateFromTask(copyAction.Execute);
        Progress = copyAction.Progress;
    }

    public string Source { get; }
    public string Destination { get; }
    public IReactiveCommand Transfer { get; }
    public IObservable<LongProgress> Progress { get; }
}