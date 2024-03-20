namespace Zafiro.Avalonia.FileExplorer.Clipboard;

public interface IClipboard
{
    void Add(IEnumerable<IClipboardItem> items);
    ReadOnlyObservableCollection<IClipboardItem> Contents { get; }
}