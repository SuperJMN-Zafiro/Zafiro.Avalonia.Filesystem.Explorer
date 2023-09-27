using System;
using System.ComponentModel;
using System.Reactive;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public interface ITransferItem : INotifyPropertyChanged
{
    ZafiroPath Source { get; }
    ZafiroPath Destination { get; }
    IStoppableCommand<Unit, Result> DoTransfer { get; }
    IObservable<LongProgress> Progress { get; }
    public IObservable<bool> IsTransferringObs { get; }
    public bool IsTransferring { get; }
}