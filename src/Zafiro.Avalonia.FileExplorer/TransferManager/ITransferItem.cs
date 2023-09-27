using System;
using System.Reactive;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public interface ITransferItem
{
    string Source { get; }
    string Destination { get; }
    IStoppableCommand DoTransfer { get; }
    IObservable<LongProgress> Progress { get; }
}