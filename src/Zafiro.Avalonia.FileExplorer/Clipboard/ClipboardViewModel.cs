using DynamicData;

namespace Zafiro.Avalonia.FileExplorer.Clipboard;

public class ClipboardViewModel : IClipboard
{
    private readonly SourceCache<IClipboardItem, string> source;

    public ClipboardViewModel()
    {
        source = new SourceCache<IClipboardItem, string>(x => x.Path);
        source
            .Connect()
            .Bind(out var contents)
            .Subscribe();

        Contents = contents;
    }

    public ReadOnlyObservableCollection<IClipboardItem> Contents { get; }

    public void Add(IEnumerable<IClipboardItem> items)
    {
        source.Edit(actions =>
        {
            actions.Load(items);
        });
    }
}
