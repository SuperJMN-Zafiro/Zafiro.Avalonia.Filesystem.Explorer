using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Dialogs;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public class ToolBarViewModel : ReactiveValidationObject
{
    public ExplorerContext Context { get; }
    private readonly ObservableAsPropertyHelper<DirectoryContentsViewModel> currentDirectory;

    public ToolBarViewModel(ExplorerContext context)
    {
        Context = context;
        currentDirectory = context.Directory.ToProperty(this, x => x.CurrentDirectory);
        
        CreateDirectory = ReactiveCommand.CreateFromTask(async () =>
        {
            var createDirectoryViewModel = new CreateDirectoryViewModel(context);
            var result = await context.Dialog.ShowAndGetResult(createDirectoryViewModel, "Create directory", x => x.IsValid(), model => model.DirectoryName, Maybe<Action<ConfigureSizeContext>>.From(x =>
            {
                x.Width = 300;
                x.Height = double.NaN;
            }));
            
            return await result.Map(name => CurrentDirectory.CreateDirectory(name));
        });

        CreateDirectory.Values().HandleErrorsWith(context.NotificationService);
        Copy = ReactiveCommand.Create(() => { });
        Paste = ReactiveCommand.Create(() => { });
        Delete = ReactiveCommand.Create(() => { });
    }

    public ReactiveCommand<Unit,Unit> Copy { get; }
    public ReactiveCommand<Unit,Unit> Paste { get; }
    public ReactiveCommand<Unit,Unit> Delete { get; }

    public ReactiveCommand<Unit, Maybe<Result<IMutableDirectory>>> CreateDirectory { get; set; }


    public DirectoryContentsViewModel CurrentDirectory => currentDirectory.Value;
    public IObservable<bool> IsPasting => Observable.Return(false);
}