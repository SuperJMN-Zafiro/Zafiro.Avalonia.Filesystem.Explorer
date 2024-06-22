using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using DynamicData;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Dialogs;
using Zafiro.CSharpFunctionalExtensions;
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
        Copy = ReactiveCommand.CreateFromTask(async () =>
        {
            var clipboard = (((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime)!).MainWindow!.Clipboard;
            var dataObject = new DataObject();
            dataObject.Set("x-special/zafiro-copied-files", Context.SelectionContext.SelectionChanges);
            await clipboard.SetDataObjectAsync(dataObject);
        });
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