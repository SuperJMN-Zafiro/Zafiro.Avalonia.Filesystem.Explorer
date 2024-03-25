using System.IO;
using System.Threading.Tasks;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IFile : IEntry
{
    long Size { get; }
    IObservable<byte> GetStream();
}