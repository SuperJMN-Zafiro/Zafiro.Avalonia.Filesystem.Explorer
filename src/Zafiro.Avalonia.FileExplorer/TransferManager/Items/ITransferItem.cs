using System;
using System.ComponentModel;
using System.Reactive;
using CSharpFunctionalExtensions;
using Zafiro.Actions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.TransferManager.Items;

public interface ITransferItem : INotifyPropertyChanged
{
    ZafiroPath Source { get; }
    ZafiroPath Destination { get; }
    IStoppableCommand<Unit, Result> DoTransfer { get; }
    IObservable<LongProgress> Progress { get; }
    public bool IsTransferring { get; }
    public IObservable<string> Errors { get; }
}