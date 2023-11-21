using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Model;

public class EverythingEntryFactory : IEntryFactory
{
    private readonly IAddress address;

    public EverythingEntryFactory(IAddress address)
    {
        this.address = address;
    }
    
    public Task<Result<IEnumerable<IEntry>>> Get(IZafiroDirectory directory)
    {
        var files = directory.GetFiles().Map(files => files.Where(x => !x.IsHidden).Select(file => (IEntry) new FileItemViewModel(file)));
        var dirs = directory.GetDirectories().Map(dirs => dirs.Where(x => !x.IsHidden).Select(dir => (IEntry) new DirectoryItemViewModel(dir, address)));

        return from f in files
            from n in dirs
            select f.Concat(n);
    }
}