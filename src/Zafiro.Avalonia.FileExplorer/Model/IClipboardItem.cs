using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IClipboardItem
{
    ZafiroPath Path { get; }
}