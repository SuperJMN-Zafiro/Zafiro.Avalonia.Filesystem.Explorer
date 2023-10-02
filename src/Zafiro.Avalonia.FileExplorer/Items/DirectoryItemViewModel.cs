using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Explorer.Address;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class DirectoryItemViewModel : ReactiveObject, IEntry
{
    public DirectoryItemViewModel(IZafiroDirectory directory, IAddress address)
    {
        Directory = directory;
        Navigate = ReactiveCommand.CreateFromTask(() => address.SetDirectory(directory.Path));
    }

    public IZafiroDirectory Directory { get; }

    public ReactiveCommand<Unit, Result<IZafiroDirectory>> Navigate { get; }

    public string Name => Path.Name();
    public ZafiroPath Path => Directory.Path;

    [Reactive] public bool IsSelected { get; set; }
}