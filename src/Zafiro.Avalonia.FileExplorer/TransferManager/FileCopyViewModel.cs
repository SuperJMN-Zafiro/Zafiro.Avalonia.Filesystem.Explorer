using System;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.FileSystem.Actions;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public class FileCopyViewModel : ReactiveObject, ITransferItem
{
    public TransferKey Key { get; }
    public IReactiveCommand Transfer { get; }

    public FileCopyViewModel(CopyFileAction copyAction)
    {
        Key = new TransferKey(copyAction.Source.Path, copyAction.Destination.Path);
        Transfer = ReactiveCommand.CreateFromTask(copyAction.Execute);
        Progress = copyAction.Progress;
    }

    public IObservable<LongProgress> Progress { get; }
}