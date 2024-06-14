using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.NextGen.Core;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.DynamicData;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;

public class PathNavigatorViewModel : ReactiveObject, IPathNavigator
{
    public PathNavigatorViewModel(IFileSystem fileSystem, INotificationService notificationService)
    {
        LoadRequestedPath = ReactiveCommand.CreateFromTask(() => RequestedPath.Map(fileSystem.Get));
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
        CurrentDirectory
            .Do(maybe => maybe.Execute(x => RequestedPathString = x.Path.ToString()))
            .Subscribe();
        
        RequestedPathString = string.Empty;
    }

    private Result<ZafiroPath> RequestedPath => RequestedPathString.Trim() == "" ? Result.Success(ZafiroPath.Empty) : ZafiroPath.Create(RequestedPathString!);

    public ReactiveCommandBase<Unit, Result<IRooted<IDynamicDirectory>>> LoadRequestedPath { get; }

    public IObservable<Maybe<IRooted<IDynamicDirectory>>> CurrentDirectory { get; }

    public ReactiveCommand<Unit, Unit> GoBack { get; set; }

    [Reactive]
    public string RequestedPathString { get; set; }

    private History History { get; }

    public IObservable<bool> IsNavigating { get; }

    public void SetAndLoad(ZafiroPath requestedPath)
    {
        History.CurrentFolder = requestedPath;
    }
}