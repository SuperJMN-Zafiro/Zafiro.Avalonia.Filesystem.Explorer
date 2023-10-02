using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
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
    private readonly IFileSystem fileSystem;
    private readonly ObservableAsPropertyHelper<ZafiroPath> path;

    public AddressViewModel(IFileSystem fileSystem, INotificationService notificationService)
    {
        this.fileSystem = fileSystem;
        History = new History(GetDefaultPath());

        GoToPath = ReactiveCommand.CreateFromTask(() => GoTo(RequestedPath!), this.WhenAnyValue(x => x.RequestedPath).NotNull());

        path = GoToPath.Successes()
            .Select(directory => directory.Path)
            .ToProperty(this, x => x.Path);

        GoToPath.Successes().Select(x => x.Path).BindTo(this, x => x.History.CurrentFolder);
       
        GoToPath.HandleErrorsWith(notificationService);

        GoBack = History.GoBack;
        IsNavigating = GoToPath.IsExecuting;

        RequestedPath = "/";
    }

    private Task<Result<IZafiroDirectory>> GoTo(string requestedPath)
    {
        return fileSystem.GetDirectory(requestedPath)
            .Tap(e => CurrentDirectory = e);
    }

    [Reactive]
    public IZafiroDirectory CurrentDirectory { get; private set; }

    public ZafiroPath Path => path.Value;

    public ReactiveCommand<Unit, Unit> GoBack { get; set; }

    [Reactive] public string RequestedPath { get; set; }

    public History History { get; }

    public ReactiveCommand<Unit, Result<IZafiroDirectory>> GoToPath { get; }

    public IObservable<bool> IsNavigating { get; }

    private static ZafiroPath GetDefaultPath()
    {
        return "/";
    }

    public Task<Result<IZafiroDirectory>> SetDirectory(ZafiroPath requestedPath)
    {
        return GoTo(requestedPath);
    }
}