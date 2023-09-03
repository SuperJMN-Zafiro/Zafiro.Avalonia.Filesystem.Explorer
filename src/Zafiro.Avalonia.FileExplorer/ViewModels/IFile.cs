using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.FileExplorer.ViewModels;

public interface IFile : IEntry
{
    long Size { get; }
    Task<Result<Stream>> GetStream();
}