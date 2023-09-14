using System;
using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class FileItemViewModel : ReactiveObject, IFile
{
    private readonly IZafiroFile file;

    public FileItemViewModel(IZafiroFile file)
    {
        this.file = file;
        IsSelected = this.WhenAnyValue(x => x.IsSelectedMutable);
    }

    [Reactive]
    public bool IsSelectedMutable { get; set; }

    public ZafiroPath Path => file.Path;

    public Task<Result<Stream>> GetStream()
    {
        return file.GetContents();
    }

    public string Name => Path.Name();

    public long Size { get; }
    public IObservable<bool> IsSelected { get; }
}