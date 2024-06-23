using System.Reactive.Linq;
using Zafiro.Actions;
using Zafiro.FileSystem.Actions;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

internal class TransferItem : ReactiveObject, ITransferItem
{
    public TransferItem(string name, ZafiroPath path, ItemType itemType, IFileAction action)
    {
        Path = path;
        Name = name;
        ItemType = itemType;
        Action = action;
        StartCommand = StoppableCommand.CreateFromTask(ct => Action.Execute(ct), Maybe.From(Observable.Return(true)));
        Progress = action.Progress;
        IsTransferring = StartCommand.Start.IsExecuting;
    }

    public IObservable<bool> IsTransferring { get; }

    public IObservable<LongProgress> Progress { get; }

    public IStoppableCommand<Unit,Result> StartCommand { get; set; }

    public string Key => Path.Combine(Name) + ItemType;
    // public Task Start(CancellationToken cancellationToken)
    // {
    //     return Action.Execute(cancellationToken);
    // }

    public ZafiroPath Path { get; }
    public string Name { get; }

    public ItemType ItemType { get; }
    public IFileAction Action { get; }
}