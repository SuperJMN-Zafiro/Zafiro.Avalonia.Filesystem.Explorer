using Zafiro.FileSystem.Core;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public interface IEntry : INamed
{
    public string Key { get;  }
}