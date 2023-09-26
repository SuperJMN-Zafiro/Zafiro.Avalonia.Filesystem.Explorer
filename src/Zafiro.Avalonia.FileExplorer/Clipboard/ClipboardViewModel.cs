using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using Zafiro.Avalonia.FileExplorer.Items;

namespace Zafiro.Avalonia.FileExplorer.Clipboard;

public class ClipboardViewModel : IClipboard
{
    private readonly SourceCache<IClipboardItem, string> source;

    public ClipboardViewModel()
    {
        source = new SourceCache<IClipboardItem, string>(x => x.Path);
        source
            .Connect()
            .Bind(out var pendingActions)
            .Subscribe();

        Entries = pendingActions;
    }

    public ReadOnlyObservableCollection<IClipboardItem> Entries { get; }

    public void Add(IEnumerable<IClipboardItem> items)
    {
        source.Edit(pendingActions =>
        {
            pendingActions.Load(items);
        });
    }
}
