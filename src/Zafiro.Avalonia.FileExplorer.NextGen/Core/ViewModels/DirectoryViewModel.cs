using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using Zafiro.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public class DirectoryViewModel: ReactiveObject, IEntry
{
    public IRooted<IMutableDirectory> Parent { get; }
    public IMutableDirectory Directory { get; }
    public ExplorerContext Context { get; }

    public DirectoryViewModel(IRooted<IMutableDirectory> parent, IMutableDirectory directory, ExplorerContext context)
    {
        Parent = parent;
        Directory = directory;
        Context = context;
        Navigate = ReactiveCommand.Create(() =>
        {
            context.PathNavigator.SetAndLoad(parent.Path.Combine(directory.Name));
        });
        
        Delete = ReactiveCommand.CreateFromObservable(() =>
        {
            return Observable.FromAsync(() => context.Dialog.ShowConfirmation($"Delete {Name}",
                $"Do you really want to delete the folder {Name}?"))
                .Trues()
                .SelectMany(_ => directory.Delete());
        });

        Delete.HandleErrorsWith(context.NotificationService);
    }

    public ReactiveCommand<Unit, Result> Delete { get; set; }

    public ReactiveCommand<Unit,Unit> Navigate { get; }

    public string Name => Directory.Name;
    public string Key => Directory.Name + "/";
}