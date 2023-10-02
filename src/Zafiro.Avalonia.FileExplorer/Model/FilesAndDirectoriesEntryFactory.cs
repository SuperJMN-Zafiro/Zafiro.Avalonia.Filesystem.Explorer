using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Model;

public class FilesAndDirectoriesEntryFactory : IDirectoriesEntryFactory
{
        public Task<Result<IEnumerable<IEntry>>> Get(IZafiroDirectory directory)
    {
        var files = directory.GetFiles().Map(files => files.Select(file => (IEntry)new FileItemViewModel(file)));
        var dirs = directory.GetDirectories().Map(dirs => dirs.Select(dir => (IEntry)new DirectoryItemViewModel(dir)));

        return from f in files
            from n in dirs
            select f.Concat(n);
    }
}