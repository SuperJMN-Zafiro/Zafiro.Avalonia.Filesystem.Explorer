using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.Mixins;

namespace Zafiro.Avalonia.FileExplorer.ViewModels;

public class FolderContentsViewModel : ViewModelBase
{
    private readonly ObservableAsPropertyHelper<DetailsViewModel> details;
    public IFileSystem FileSystem { get; }

    public FolderContentsViewModel(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
        History = new History<ZafiroPath>(GetDefaultPath());

        Path = GetDefaultPath();
        GoToPath = ReactiveCommand.CreateFromTask(() => fileSystem.GetDirectory(RequestedPath!), this.WhenAnyValue(x => x.RequestedPath).NotNull());

        details = GoToPath.Successes()
            .Select(directory => new DetailsViewModel(directory))
            .ToProperty(this, model => model.Details);

        GoToPath.Successes()
            .Select(directory => directory.Path)
            .ToProperty(this, model => model.Path);

        GoToPath.Successes().Select(x => x.Path).BindTo(this, x => x.History.CurrentFolder);
        this.WhenAnyObservable(x => x.Details.SelectedItems).OfType<FolderItemViewModel>()
            .Select(x => x.Path)
            .Merge(this.WhenAnyValue(x => x.History.CurrentFolder))
            .Do(activatedFolder => RequestedPath = activatedFolder.Path)
            .ToSignal()
            .InvokeCommand(GoToPath);

        GoBack = History.GoBack;
        IsNavigating = GoToPath.IsExecuting.CombineLatest(this.WhenAnyObservable(model => model.Details.IsLoadingChildren), (b, b1) => b || b1);
    }

    public ReactiveCommand<Unit, Unit> GoBack { get; set; }

    [Reactive]
    public string RequestedPath { get; set; }

    public History<ZafiroPath> History { get; set; }

    public DetailsViewModel Details => details.Value;

    public ReactiveCommand<Unit, Result<IZafiroDirectory>> GoToPath { get; }

    public ZafiroPath Path { get; private set; }

    public IObservable<bool> IsNavigating { get; }

    private static ZafiroPath GetDefaultPath()
    {
        return "/";
    }
}

public interface IHistory<T>
{
    ReactiveCommand<Unit, Unit> GoBack { get; }
    T CurrentFolder { get; set; }
    Maybe<T> PreviousFolder { get; }
}

public class History<T> : ReactiveObject, IHistory<T>
{
    private readonly Stack<T> currentFolderStack;

    public History(T initial)
    {
        currentFolderStack = new Stack<T>(new[] { initial });
        var whenAnyValue = this.WhenAnyValue(x => x.CanGoBack);
        GoBack = ReactiveCommand.Create(OnBack, whenAnyValue);
    }

    private bool CanGoBack => currentFolderStack.Count > 1;

    public ReactiveCommand<Unit, Unit> GoBack { get; }

    public T CurrentFolder
    {
        get => currentFolderStack.Peek();
        set
        {
            if (Equals(value, currentFolderStack.Peek()))
            {
                return;
            }

            currentFolderStack.Push(value);
            this.RaisePropertyChanged(nameof(CanGoBack));
            this.RaisePropertyChanged(nameof(PreviousFolder));
            this.RaisePropertyChanged();
        }
    }

    public Maybe<T> PreviousFolder => currentFolderStack.SkipLast(1).TryFirst();

    private void OnBack()
    {
        currentFolderStack.Pop();
        this.RaisePropertyChanged(nameof(CurrentFolder));
        this.RaisePropertyChanged(nameof(PreviousFolder));
        this.RaisePropertyChanged(nameof(CanGoBack));
    }
}