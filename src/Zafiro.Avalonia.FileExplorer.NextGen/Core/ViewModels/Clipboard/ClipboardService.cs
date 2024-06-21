using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using Avalonia.Input;
using Avalonia.Input.Platform;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.DataModel;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;
using IFile = Zafiro.FileSystem.Readonly.IFile;
using IFileSystem = Zafiro.FileSystem.Mutable.IFileSystem;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Clipboard;

public class ClipboardService : IClipboardService
{
    private const string MimeType = "x-special/zafiro-copied-files";

    public ClipboardService(IClipboard clipboard, ITransferManager transferManager, IDictionary<string, Zafiro.FileSystem.Mutable.IFileSystem> fileSystems)
    {
        Clipboard = clipboard;
        TransferManager = transferManager;
        FileSystems = fileSystems;
    }

    public IClipboard Clipboard { get; }
    public ITransferManager TransferManager { get; }
    public IDictionary<string, IFileSystem> FileSystems { get; }

    public async Task<Result> Copy(IEnumerable<IDirectoryItem> items, ZafiroPath sourcePath, IFileSystem fileSystem)
    {
        var serialized = Serialize(items, sourcePath, fileSystem);
        var dataObject = new DataObject();
        dataObject.Set(MimeType, serialized);
        await Clipboard.SetDataObjectAsync(dataObject);
        return Result.Success();
    }

    public Task<Result> Paste(IMutableDirectory destination)
    {
        var data = Result.Try(() => Clipboard.GetDataAsync(MimeType))
            .Map(o => (byte[]?)o!)
            .Map(bytes => Encoding.UTF8.GetString(bytes))
            .Map(s => JsonSerializer.Deserialize<List<CopiedClipboardEntry>>(s))
            .Bind(list => Paste(list!, destination));

        return data;
    }

    private string Serialize(IEnumerable<IDirectoryItem> selectedItems, ZafiroPath parentPath, IFileSystem fileSystem)
    {
        var toSerializationModel = ToSerializationModel(selectedItems, parentPath);
        return JsonSerializer.Serialize(toSerializationModel);
    }

    private IEnumerable<CopiedClipboardEntry> ToSerializationModel(IEnumerable<IDirectoryItem> selectedItems,
        ZafiroPath parentPath)
    {
        return selectedItems.Select(x => new CopiedClipboardEntry(x.Name, parentPath, "local", x is FileViewModel ? ItemType.File : ItemType.Directory));
    }

    public async Task<Result> Paste(List<CopiedClipboardEntry> items, IMutableDirectory destination)
    {
        var combine = items.Select(x => Item(x, destination)).Combine();
        var result = await combine;
        result.Tap(enumerable =>
        {
            TransferManager.Add(enumerable.ToArray());
        });
        return result;
    }

    private Task<Result<ITransferItem>> Item(CopiedClipboardEntry entry, IMutableDirectory directory)
    {
        var source = FromEntry(entry);
        var destination = directory.CreateFile(entry.Name);

        var copyAction = source.CombineAndMap(destination, (src, dst) => new CopyFileAction(src, dst));
        return copyAction.Map(action => (ITransferItem) new TransferItem(entry.Name, entry.ParentPath, entry.Type, action));
    }

    private Task<Result<IFile>> FromEntry(CopiedClipboardEntry entry)
    {
        var folder = FileSystems[entry.FileSystemKey].Get((ZafiroPath)entry.ParentPath).Map(x => x.Value);
        return folder.Bind(x => x.Files().Bind(x => x.TryFirst(x => Equals(x.Name, entry.Name)).ToResult("Not found")));
    }
}