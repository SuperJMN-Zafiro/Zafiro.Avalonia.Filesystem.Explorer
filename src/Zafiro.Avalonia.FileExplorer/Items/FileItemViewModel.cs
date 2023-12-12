using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class FileItemViewModel : ReactiveObject, IFile
{
    public IZafiroFile File { get; }

    public FileItemViewModel(IZafiroFile file)
    {
        File = file;
    }

    public ZafiroPath Path => File.Path;

    public IObservable<byte> GetStream()
    {
        return File.Contents;
    }

    public string Name => Path.Name();

    public long Size { get; }

    [Reactive]
    public bool IsSelected { get; set; }
}