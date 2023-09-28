using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class ClipboardDirectoryItemViewModel : IClipboardItem
{
    public IZafiroDirectory Directory { get; }

    public ClipboardDirectoryItemViewModel(IZafiroDirectory directory)
    {
        this.Directory = directory;
    }

    public ZafiroPath Path => Directory.Path;
}