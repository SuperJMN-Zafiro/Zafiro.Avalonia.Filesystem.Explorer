using System;
using Zafiro.Actions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public interface ITransferItem
{
    ZafiroPath Source { get; }
    ZafiroPath Destination { get; }
    IStoppableCommand DoTransfer { get; }
    IObservable<LongProgress> Progress { get; }
}