﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Actions;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Actions;
using Zafiro.Mixins;
using Zafiro.UI;
using Observable = System.Reactive.Linq.Observable;

namespace Zafiro.Avalonia.FileExplorer.TransferManager.Items;

public class FileDeleteViewModel : ReactiveObject, ITransferItem
{
    public FileDeleteViewModel(DeleteFileAction deleteFileAction)
    {
        Source = deleteFileAction.Source.Path;
        DoTransfer = StoppableCommand.CreateFromTask(deleteFileAction.Execute, Maybe<IObservable<bool>>.None);
        Progress = deleteFileAction.Progress;
        DoTransfer.IsExecuting.BindTo(this, x => x.IsTransferring);
        Errors = DoTransfer.Start.Failures();
        EstimatedCompletion = Observable.Return(Maybe<TimeSpan>.None);
    }

    public IObservable<Maybe<TimeSpan>> EstimatedCompletion { get; }
    public string Description => $"Deleting {Source}";
    public ZafiroPath Source { get; }
    public ZafiroPath Destination { get; }
    public IStoppableCommand<Unit, Result> DoTransfer { get; }
    public IObservable<LongProgress> Progress { get; }

    [Reactive] public bool IsTransferring { get; private set; }

    public IObservable<string> Errors { get; }
}