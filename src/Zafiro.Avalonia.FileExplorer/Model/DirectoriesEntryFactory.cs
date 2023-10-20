using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Model;

public class DirectoriesEntryFactory : IEntryFactory
{
    private readonly IAddress address;

    public DirectoriesEntryFactory(IAddress address)
    {
        this.address = address;
    }

    public Task<Result<IEnumerable<IEntry>>> Get(IZafiroDirectory directory)
    {
        return directory.GetDirectories().Map(dirs => dirs.Select(dir => (IEntry)new DirectoryItemViewModel(dir, address)));
    }
}