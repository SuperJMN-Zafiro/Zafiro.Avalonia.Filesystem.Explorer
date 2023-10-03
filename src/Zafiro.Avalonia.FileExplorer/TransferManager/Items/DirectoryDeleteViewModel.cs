using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.TransferManager.Items;

public class DirectoryDeleteViewModel : ReactiveObject, ITransferItem
{
    public DirectoryDeleteViewModel(DeleteDirectoryAction deleteDirectoryAction)
    {
        Source = deleteDirectoryAction.Directory.Path;
        DoTransfer = StoppableCommand.CreateFromTask(deleteDirectoryAction.Execute, Observable.Return(true));
        Progress = deleteDirectoryAction.Progress;
        DoTransfer.IsExecuting.DelayItem(false, TimeSpan.FromSeconds(5)).BindTo(this, x => x.IsTransferring);
        Errors = DoTransfer.Start.Failures();
    }

    public string Description => $"Delete {Source}";
    public ZafiroPath Source { get; }
    public ZafiroPath Destination { get; }
    public IStoppableCommand<Unit, Result> DoTransfer { get; }
    public IObservable<LongProgress> Progress { get; }
    public IObservable<bool> IsTransferringObs => DoTransfer.IsExecuting;

    [Reactive]
    public bool IsTransferring { get; private set; }

    public IObservable<string> Errors { get; }
}