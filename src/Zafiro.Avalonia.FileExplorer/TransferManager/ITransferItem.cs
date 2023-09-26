using System;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public interface ITransferItem
{
    ZafiroPath Source { get; }
    ZafiroPath Destination { get; }
    IReactiveCommand Transfer { get; }
    IObservable<LongProgress> Progress { get; }
}