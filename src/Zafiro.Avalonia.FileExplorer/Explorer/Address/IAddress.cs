using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Explorer.Address;

public interface IAddress
{
    Task<Result<IZafiroDirectory>> SetDirectory(ZafiroPath requestedPath);
}