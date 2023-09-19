using System.Collections.Generic;
using Zafiro.Avalonia.FileExplorer.Items;

namespace Zafiro.Avalonia.FileExplorer.ViewsModes.FolderContents;

public interface IPendingActionsManager
{
    void Copy(IEnumerable<IClipboardItem> items);
}