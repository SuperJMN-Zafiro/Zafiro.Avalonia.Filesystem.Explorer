using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.ViewModels;

public class FileViewModel : ViewModelBase, IFile
{
    private readonly IZafiroFile file;

    public FileViewModel(IZafiroFile file)
    {
        this.file = file;
    }

    public ZafiroPath Path => file.Path;

    public Task<Result<Stream>> GetStream()
    {
        return file.GetContents();
    }

    public string Name => Path.Name();

    public long Size { get; }
}