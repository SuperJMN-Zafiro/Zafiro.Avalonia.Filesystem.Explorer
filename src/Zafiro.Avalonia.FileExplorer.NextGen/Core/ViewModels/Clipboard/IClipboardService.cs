using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Clipboard;

public interface IClipboardService
{
    public Task<Result> Copy(IEnumerable<IDirectoryItem> items, ZafiroPath sourcePath, IFileSystem fileSystem);
    public Task<Result> Paste();
}