using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.ViewModels;

internal class FolderItemViewModel : ViewModelBase, IEntry
{
    private readonly IZafiroDirectory dir;

    public FolderItemViewModel(IZafiroDirectory dir)
    {
        this.dir = dir;
    }

    public ZafiroPath Path => dir.Path;
    public string Name => Path.Name();
}