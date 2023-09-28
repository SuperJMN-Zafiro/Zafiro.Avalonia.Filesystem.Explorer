using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public interface IClipboardItem
{
    ZafiroPath Path { get; }
}