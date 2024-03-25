using System.Threading.Tasks;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IEntryFactory
{
    Task<Result<IEnumerable<IEntry>>> Get(IZafiroDirectory directory);
}