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
        LoadRequestedPath.Successes().BindTo(this, x => x.CurrentDirectory);
        IsNavigating = LoadRequestedPath.IsExecuting;
        var root = fileSystem.GetRoot();
        History = new History(root);
        LoadRequestedPath.Successes().Select(x => x.Path).BindTo(this, x => x.History.CurrentFolder);
        GoBack = History.GoBack;
        this.WhenAnyValue(x => x.History.CurrentFolder).Do(SetRequestedPath).Subscribe();
        RequestedPathString = root;
    }

    private Result<ZafiroPath> RequestedPath => RequestedPathString?.Trim() == "" ? Result.Success(ZafiroPath.Empty) : ZafiroPath.Create(RequestedPathString!);

    public ReactiveCommand<Unit, Result<IZafiroDirectory>> LoadRequestedPath { get; set; }

    [Reactive] public IZafiroDirectory CurrentDirectory { get; set; }

    public ReactiveCommand<Unit, Unit> GoBack { get; set; }

    [Reactive] public string? RequestedPathString { get; set; }

    private History History { get; }

    public IObservable<bool> IsNavigating { get; }

    public void SetRequestedPath(ZafiroPath requestedPath)
    {
        RequestedPathString = requestedPath;
        LoadRequestedPath.Execute().Take(1).Subscribe();
    }
}