using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable.Mutable;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public class CreateDirectoryViewModel : ReactiveValidationObject
{
    private readonly ObservableAsPropertyHelper<ReactiveCommand<Unit, Result<IMutableDirectory>>> command;
    private readonly CompositeDisposable disposable = new();

    public CreateDirectoryViewModel(ExplorerContext context)
    {
        this.ValidationRule(x => x.DirectoryName, s => !string.IsNullOrEmpty(s), "Can't be empty");
        var directories = context.PathNavigator.Directories.Values();
        var commands = directories.Select(dir =>
            ReactiveCommand.CreateFromTask(() => dir.Value.CreateDirectory(DirectoryName!),
                this.WhenAnyValue(x => x.DirectoryName).NotNull()));
        command = commands
            .ToProperty(this, x => x.CreateNewDirectory);

        directories
            .Do(_ => DirectoryName = "")
            .Subscribe()
            .DisposeWith(disposable);
        
        IsBusy = commands.Select(x => x.IsExecuting).Switch();
    }

    public IObservable<bool> IsBusy { get; }

    [Reactive] public string DirectoryName { get; set; } = "";

    public ReactiveCommand<Unit, Result<IMutableDirectory>> CreateNewDirectory => command.Value;

    protected override void Dispose(bool disposing)
    {
        CreateNewDirectory.Dispose();
        command.Dispose();
        disposable.Dispose();
    }
}