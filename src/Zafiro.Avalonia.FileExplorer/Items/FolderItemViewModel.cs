using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class FolderItemViewModel : ReactiveObject, IEntry
{
    private readonly IZafiroDirectory dir;

    public FolderItemViewModel(IZafiroDirectory dir)
    {
        this.dir = dir;
        IsSelected = this.WhenAnyValue(x => x.IsSelectedMutable);
    }

    [Reactive] public bool IsSelectedMutable { get; set; }
    public string Name => Path.Name();
    public ZafiroPath Path => dir.Path;
    public IObservable<bool> IsSelected { get; }
}