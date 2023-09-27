using System;
using ReactiveUI;
using Zafiro.Actions;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public interface ITransferItem
{
    string Source { get; }
    string Destination { get; }
    IReactiveCommand Transfer { get; }
    IObservable<LongProgress> Progress { get; }
}