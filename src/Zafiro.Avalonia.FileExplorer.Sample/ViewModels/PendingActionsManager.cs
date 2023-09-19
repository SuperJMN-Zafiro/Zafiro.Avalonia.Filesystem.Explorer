using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DynamicData;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.Avalonia.FileExplorer.ViewsModes.FolderContents;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class PendingActionsManager : IPendingActionsManager
{
    private readonly SourceCache<IClipboardItem, string> source;

    public PendingActionsManager()
    {
        source = new SourceCache<IClipboardItem, string>(x => x.Path);
        source
            .Connect()
            .Bind(out var pendingActions)
            .Subscribe();

        Entries = pendingActions;
    }

    public ReadOnlyObservableCollection<IClipboardItem> Entries { get; }

    public void Copy(IEnumerable<IClipboardItem> items)
    {
        source.Edit(pendingActions =>
        {
            pendingActions.Load(items);
        }); 
    }
}
