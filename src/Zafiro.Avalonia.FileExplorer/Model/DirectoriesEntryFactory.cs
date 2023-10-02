using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IDirectoriesEntryFactory
{
    Task<Result<IEnumerable<IEntry>>> Get(IZafiroDirectory directory);
}

public class DirectoriesEntryFactory : IDirectoriesEntryFactory
{
    public Task<Result<IEnumerable<IEntry>>> Get(IZafiroDirectory directory)
    {
        return directory.GetDirectories().Map(dirs => dirs.Select(dir => (IEntry)new DirectoryItemViewModel(dir)));
    }
}