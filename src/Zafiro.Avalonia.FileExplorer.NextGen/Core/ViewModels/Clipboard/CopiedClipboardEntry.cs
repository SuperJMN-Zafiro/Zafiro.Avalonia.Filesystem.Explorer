using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;
using Zafiro.FileSystem.Core;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Clipboard;

public class CopiedClipboardEntry
{
    public CopiedClipboardEntry(string name, string parentPath, string fileSystemKey, ItemType type)
    {
        Name = name;
        ParentPath = parentPath;
        FileSystemKey = fileSystemKey;
        Type = type;
    }

    public string Name { get;  }
    public string ParentPath { get; }
    public string FileSystemKey { get; }
    public ItemType Type { get; }
}