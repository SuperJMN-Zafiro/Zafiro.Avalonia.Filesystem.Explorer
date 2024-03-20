using System.Linq;
using System.Threading.Tasks;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.Items;

namespace Zafiro.Avalonia.FileExplorer.Model;

public class EverythingEntryFactory : IEntryFactory
{
    private readonly IPathNavigator pathNavigator;
    private readonly IContentOpener opener;
    private readonly INotificationService notificationService;
    private readonly ISelectionContext selectionContext;

    public EverythingEntryFactory(IPathNavigator pathNavigator, IContentOpener opener, INotificationService notificationService, ISelectionContext selectionContext)
    {
        this.pathNavigator = pathNavigator;
        this.opener = opener;
        this.notificationService = notificationService;
        this.selectionContext = selectionContext;
    }

    public Task<Result<IEnumerable<IEntry>>> Get(IZafiroDirectory directory)
    {
        var filesWithProperties = WithProperties(directory.GetFiles());
        var dirsWithProperties = WithProperties(directory.GetDirectories());
        
        var files = filesWithProperties
            .Map(files => files.Where(x => !x.Item2.IsHidden)
                .Select(file => (IEntry) new FileItemViewModel(file.Item1, file.Item2, opener, selectionContext, notificationService)));

        var dirs = dirsWithProperties
            .Map(dirs => dirs.Where(x => !x.Item2.IsHidden)
                .Select(dir => (IEntry) new DirectoryItemViewModel(dir.Item1, pathNavigator)));

        return from f in files
            from n in dirs
            select f.Concat(n);
    }

    private async Task<Result<IEnumerable<(IZafiroFile, FileProperties)>>> WithProperties(Task<Result<IEnumerable<IZafiroFile>>> files)
    {
        var withProperties = await files.Bind(async zafiroFiles =>
        {
            var results = await Task.WhenAll(zafiroFiles.Select(async file =>
            {
                var fileProperties = await file.Properties;
                return fileProperties.Map(p => (file, p));
            })).ConfigureAwait(false);

            return results.Combine();
        }).ConfigureAwait(false);

        return withProperties;
    }

    private async Task<Result<IEnumerable<(IZafiroDirectory, DirectoryProperties)>>> WithProperties(Task<Result<IEnumerable<IZafiroDirectory>>> dirs)
    {
        var withProperties = await dirs.Bind(async enumerable =>
        {
            var whenAll = await Task.WhenAll(enumerable.Select(async dir =>
            {
                var fileProperties = await dir.Properties;
                return fileProperties.Map(p => (file: dir, p));
            })).ConfigureAwait(false);

            return whenAll.Combine();
        }).ConfigureAwait(false);

        return withProperties;
    }
}