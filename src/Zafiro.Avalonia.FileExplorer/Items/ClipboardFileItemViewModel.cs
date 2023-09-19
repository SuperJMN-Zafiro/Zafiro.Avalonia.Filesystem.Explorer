using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class ClipboardFileItemViewModel : IClipboardItem
{
    private readonly IZafiroFile file;

    public ClipboardFileItemViewModel(IZafiroFile file)
    {
        this.file = file;
    }

    public ZafiroPath Path => file.Path;
}