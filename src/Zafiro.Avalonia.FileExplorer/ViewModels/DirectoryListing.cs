using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.ViewModels;

public class DirectoryListing
{
    public static Task<Result<IEnumerable<IEntry>>> GetAll(IZafiroDirectory directory)
    {
        var files = directory.GetFiles().Map(files => files.Select(file => (IEntry) new FileViewModel(file)));
        var dirs = GetDirectories(directory);

        return from f in files
            from n in dirs
            select f.Concat(n);
    }

    public static Task<Result<IEnumerable<IEntry>>> GetDirectories(IZafiroDirectory directory)
    {
        return directory.GetDirectories().Map(dirs => dirs.Select(dir => (IEntry) new FolderItemViewModel(dir)));
    }

    public delegate Task<Result<IEnumerable<IEntry>>> Strategy(IZafiroDirectory directory);
}