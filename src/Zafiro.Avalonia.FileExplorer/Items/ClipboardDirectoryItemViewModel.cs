using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class ClipboardDirectoryItemViewModel : IClipboardItem
{
    private readonly IZafiroDirectory directory;

    public ClipboardDirectoryItemViewModel(IZafiroDirectory directory)
    {
        this.directory = directory;
    }

    public ZafiroPath Path => directory.Path;
}