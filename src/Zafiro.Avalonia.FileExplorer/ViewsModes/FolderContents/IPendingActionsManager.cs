using System.Collections.Generic;
using System.Collections.ObjectModel;
using Zafiro.Avalonia.FileExplorer.Items;

namespace Zafiro.Avalonia.FileExplorer.ViewsModes.FolderContents;

public interface IPendingActionsManager
{
    void Copy(IEnumerable<IClipboardItem> items);
    ReadOnlyObservableCollection<IClipboardItem> Entries { get; }
}