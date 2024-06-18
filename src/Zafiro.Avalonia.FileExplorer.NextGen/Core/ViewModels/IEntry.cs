using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;

public interface IEntry : INamed
{
    public string Key { get;  }
}