using System.Text;
using System.Text.Json;
using Avalonia.Input;
using Avalonia.Input.Platform;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Clipboard;

public class ClipboardService : IClipboardService
{
    private const string MimeType = "x-special/zafiro-copied-files";

    public ClipboardService(IClipboard clipboard, ITransferManager transferManager)
    {
        Clipboard = clipboard;
        TransferManager = transferManager;
    }

    public IClipboard Clipboard { get; }
    public ITransferManager TransferManager { get; }

    public async Task<Result> Copy(IEnumerable<IDirectoryItem> items, ZafiroPath sourcePath, IFileSystem fileSystem)
    {
        var serialized = Serialize(items, sourcePath, fileSystem);
        var dataObject = new DataObject();
        dataObject.Set(MimeType, serialized);
        await Clipboard.SetDataObjectAsync(dataObject);
        return Result.Success();
    }

    public Task<Result> Paste()
    {
        var data = Result.Try(() => Clipboard.GetDataAsync(MimeType))
            .Map(o => (byte[]?)o!)
            .Map(bytes => Encoding.UTF8.GetString(bytes))
            .Map(s => JsonSerializer.Deserialize<List<CopiedClipboardEntry>>(s))
            .Bind(Paste!);

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

    private Task<Result> Paste(List<CopiedClipboardEntry> items)
    {
        items.ForEach(entry => TransferManager.Add(Item(entry)));

        return Task.FromResult(Result.Success());
    }

    private ITransferItem Item(CopiedClipboardEntry entry)
    {
        return new TransferItem(entry.Name, entry.ParentPath, entry.Type);
    }
}