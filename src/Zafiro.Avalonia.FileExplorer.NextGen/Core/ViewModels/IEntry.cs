using Zafiro.FileSystem.Core;

namespace Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;

public interface IEntry : INamed
{
    public string Key { get;  }
}