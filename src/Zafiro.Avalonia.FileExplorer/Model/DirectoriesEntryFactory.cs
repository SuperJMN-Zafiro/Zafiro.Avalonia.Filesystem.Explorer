using System.Linq;
using System.Threading.Tasks;
using Zafiro.Avalonia.FileExplorer.Items;

namespace Zafiro.Avalonia.FileExplorer.Model;

public class DirectoriesEntryFactory : IEntryFactory
{
    private readonly IPathNavigator pathNavigator;

    public DirectoriesEntryFactory(IPathNavigator pathNavigator)
    {
        this.pathNavigator = pathNavigator;
    }

    public Task<Result<IEnumerable<IEntry>>> Get(IZafiroDirectory directory)
    {
        return directory.GetDirectories().Map(dirs => dirs.Select(dir => (IEntry)new DirectoryItemViewModel(dir, pathNavigator)));
    }
}