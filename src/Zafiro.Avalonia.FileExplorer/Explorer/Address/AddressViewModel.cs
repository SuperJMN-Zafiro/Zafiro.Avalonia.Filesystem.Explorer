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
        LoadRequestedPath = ReactiveCommand.CreateFromTask(() => ZafiroPath.Create(RequestedPath!).Bind(fileSystem.GetDirectory), this.WhenAnyValue(x => x.RequestedPath).NotNull());
        LoadRequestedPath.HandleErrorsWith(notificationService);
        LoadRequestedPath.Successes().BindTo(this, x => x.CurrentDirectory);
        IsNavigating = LoadRequestedPath.IsExecuting;
        var root = fileSystem.GetRoot();
        History = new History(root);
        LoadRequestedPath.Successes().Select(x => x.Path).BindTo(this, x => x.History.CurrentFolder);
        GoBack = History.GoBack;
        this.WhenAnyValue(x => x.History.CurrentFolder).Do(SetRequestedPath).Subscribe();
        RequestedPath = root;
    }

    public ReactiveCommand<Unit, Result<IZafiroDirectory>> LoadRequestedPath { get; set; }

    [Reactive]
    public IZafiroDirectory CurrentDirectory { get; private set; }
    
    public ReactiveCommand<Unit, Unit> GoBack { get; set; }

    [Reactive] public string? RequestedPath { get; set; }

    private History History { get; }
    
    public IObservable<bool> IsNavigating { get; }

    public void SetRequestedPath(ZafiroPath requestedPath)
    {
        RequestedPath = requestedPath;
        LoadRequestedPath.Execute().Take(1).Subscribe();
    }
}