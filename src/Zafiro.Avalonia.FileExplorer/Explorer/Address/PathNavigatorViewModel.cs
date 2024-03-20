using Zafiro.Reactive;

namespace Zafiro.Avalonia.FileExplorer.Explorer.Address;

public class PathNavigatorViewModel : ReactiveObject, IPathNavigator
{
    public PathNavigatorViewModel(IFileSystemRoot fileSystem, INotificationService notificationService)
    {
        LoadRequestedPath = ReactiveCommand.Create(() => RequestedPath.Map(fileSystem.GetDirectory));
        LoadRequestedPath.HandleErrorsWith(notificationService);
        IsNavigating = LoadRequestedPath.IsExecuting;
        History = new History();
        LoadRequestedPath.Successes().Select(x => x.Path).Select(Maybe.From).BindTo(this, x => x.History.CurrentFolder);
        GoBack = History.GoBack;
        this.WhenAnyValue(x => x.History.CurrentFolder).Values()
            .Do(path => RequestedPathString = path)
            .ToSignal()
            .InvokeCommand(LoadRequestedPath);
        CurrentDirectory = LoadRequestedPath.Successes().Select(Maybe.From);
        RequestedPathString = string.Empty;
    }

    private Result<ZafiroPath> RequestedPath => RequestedPathString.Trim() == "" ? Result.Success(ZafiroPath.Empty) : ZafiroPath.Create(RequestedPathString!);

    public ReactiveCommandBase<Unit, Result<IZafiroDirectory>> LoadRequestedPath { get; }

    public IObservable<Maybe<IZafiroDirectory>> CurrentDirectory { get; }

    public ReactiveCommand<Unit, Unit> GoBack { get; set; }

    [Reactive] public string RequestedPathString { get; set; }

    private History History { get; }

    public IObservable<bool> IsNavigating { get; }

    public void SetAndLoad(ZafiroPath requestedPath)
    {
        History.CurrentFolder = requestedPath;
    }
}