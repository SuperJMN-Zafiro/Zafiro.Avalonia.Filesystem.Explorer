using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.Mixins;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Explorer.Address;

public class AddressViewModel : ReactiveObject, IAddress
{
    public AddressViewModel(IFileSystem fileSystem, INotificationService notificationService)
    {
        LoadRequestedPath = ReactiveCommand.CreateFromTask(() => RequestedPath.Bind(fileSystem.GetDirectory), this.WhenAnyValue(x => x.RequestedPathString).NotNull());
        LoadRequestedPath.HandleErrorsWith(notificationService);
        IsNavigating = LoadRequestedPath.IsExecuting;
        History = new History();
        LoadRequestedPath.Successes().Select(x => x.Path).Select(Maybe.From).BindTo(this, x => x.History.CurrentFolder);
        GoBack = History.GoBack;
        this.WhenAnyValue(x => x.History.CurrentFolder).Values().Do(SetAndLoad).Subscribe();
        CurrentDirectory = LoadRequestedPath.Successes().Select(Maybe.From);
    }

    private Result<ZafiroPath> RequestedPath => RequestedPathString?.Trim() == "" ? Result.Success(ZafiroPath.Empty) : ZafiroPath.Create(RequestedPathString!);

    public ReactiveCommand<Unit, Result<IZafiroDirectory>> LoadRequestedPath { get; set; }

    public IObservable<Maybe<IZafiroDirectory>> CurrentDirectory { get; }

    public ReactiveCommand<Unit, Unit> GoBack { get; set; }

    [Reactive] public string? RequestedPathString { get; private set; }

    private History History { get; }

    public IObservable<bool> IsNavigating { get; }

    public void SetAndLoad(ZafiroPath requestedPath)
    {
        RequestedPathString = requestedPath;
        LoadRequestedPath.Execute().Take(1).Subscribe();
    }
}