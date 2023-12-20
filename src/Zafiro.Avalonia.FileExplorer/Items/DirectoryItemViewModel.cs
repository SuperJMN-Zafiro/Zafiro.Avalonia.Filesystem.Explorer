using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class DirectoryItemViewModel : ReactiveObject, IEntry
{
    public DirectoryItemViewModel(IZafiroDirectory directory, IPathNavigator pathNavigator)
    {
        Directory = directory;
        Navigate = ReactiveCommand.Create(() => pathNavigator.SetAndLoad(directory.Path));
    }

    public IZafiroDirectory Directory { get; }

    public ReactiveCommand<Unit, Unit> Navigate { get; }

    public string Name => Path.Name();
    public ZafiroPath Path => Directory.Path;

    [Reactive] public bool IsSelected { get; set; }
}