﻿using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class ClipboardFileItemViewModel : IClipboardItem
{
    public ClipboardFileItemViewModel(IZafiroFile file)
    {
        File = file;
    }

    public ZafiroPath Path => File.Path;
    public IZafiroFile File { get; }
}