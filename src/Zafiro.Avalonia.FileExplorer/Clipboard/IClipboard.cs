using System.Collections.Generic;
using System.Collections.ObjectModel;
using Zafiro.Avalonia.FileExplorer.Model;

namespace Zafiro.Avalonia.FileExplorer.Clipboard;

public interface IClipboard
{
    void Add(IEnumerable<IClipboardItem> items);
    ReadOnlyObservableCollection<IClipboardItem> Contents { get; }
}