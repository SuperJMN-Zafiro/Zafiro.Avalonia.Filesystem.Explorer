using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class FolderItemViewModel : ReactiveObject, IEntry
{
    public IZafiroDirectory Directory { get; }

    public FolderItemViewModel(IZafiroDirectory directory)
    {
        this.Directory = directory;
    }

    public string Name => Path.Name();
    public ZafiroPath Path => Directory.Path;

    [Reactive]
    public bool IsSelected { get; set; }
}