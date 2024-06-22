namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Clipboard;

public interface IClipboardService
{
    public Task<Result> Copy(IEnumerable<IDirectoryItem> items, ZafiroPath sourcePath, IMutableFileSystem mutableFileSystem);
    public Task<Result> Paste(IMutableDirectory destination);
}