using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.ViewModels;

public interface IEntry
{
    public ZafiroPath Path { get; }
}