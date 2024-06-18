using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Dialogs;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Mutable.Mutable;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public class ToolBarViewModel : ReactiveValidationObject
{
    private readonly ObservableAsPropertyHelper<IRooted<IMutableDirectory>> currentDirectory;

    public ToolBarViewModel(ExplorerContext context)
    {
        currentDirectory = context.PathNavigator.Directories.Values().ToProperty(this, x => x.CurrentDirectory);
        
        CreateDirectory = ReactiveCommand.CreateFromTask(async () =>
        {
            var createDirectoryViewModel = new CreateDirectoryViewModel(context);
            var result = await context.Dialog.ShowAndGetResult(createDirectoryViewModel, "Create directory", x => x.IsValid(), model => model.DirectoryName, Maybe<Action<ConfigureSizeContext>>.From(x =>
            {
                x.Width = 300;
                x.Height = double.NaN;
            }));
            return await result.Map(name => CurrentDirectory.Value.CreateDirectory(name));
        });

        CreateDirectory.Values().HandleErrorsWith(context.NotificationService);
    }

    public ReactiveCommand<Unit, Maybe<Result<IMutableDirectory>>> CreateDirectory { get; set; }


    public IRooted<IMutableDirectory> CurrentDirectory => currentDirectory.Value;
}