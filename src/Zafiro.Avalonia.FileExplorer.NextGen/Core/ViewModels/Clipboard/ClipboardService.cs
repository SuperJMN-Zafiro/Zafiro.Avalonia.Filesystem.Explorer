using System.Text;
using System.Text.Json;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;
using IFile = Zafiro.FileSystem.Readonly.IFile;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Clipboard;

public class ClipboardService : IClipboardService
{
    private const string MimeType = "x-special/zafiro-copied-files";

    public ClipboardService(IClipboard clipboard, ITransferManager transferManager, IDictionary<string, Zafiro.FileSystem.Mutable.IMutableFileSystem> fileSystems)
    {
        Clipboard = clipboard;
        TransferManager = transferManager;
        FileSystems = fileSystems;
    }

    public IClipboard Clipboard { get; }
    public ITransferManager TransferManager { get; }
    public IDictionary<string, IMutableFileSystem> FileSystems { get; }

    public async Task<Result> Copy(IEnumerable<IDirectoryItem> items, ZafiroPath sourcePath, IMutableFileSystem mutableFileSystem)
    {
        var serialized = Serialize(items, sourcePath);
        var dataObject = new DataObject();
        dataObject.Set(MimeType, serialized);
        await Clipboard.SetDataObjectAsync(dataObject);
        return Result.Success();
    }

    public Task<Result> Paste(IMutableDirectory destination)
    {
        var data = Result.Try(() => Clipboard.GetDataAsync(MimeType))
            .EnsureNotNull("Nothing to paste")
            .Map(o => (byte[]?)o!)
            .Map(bytes => Encoding.UTF8.GetString(bytes))
            .Map(s => JsonSerializer.Deserialize<List<CopiedClipboardEntry>>(s))
            .Bind(list => Paste(list!, destination));

        return data;
    }

    private string Serialize(IEnumerable<IDirectoryItem> selectedItems, ZafiroPath parentPath)
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
        var transferItemResult = await GetAction(items, destination)
            .Map(action => (ITransferItem)new TransferItem($"Copiando {action.Actions.Count} elementos a {destination}", action));
        
        transferItemResult.Tap(transferItem =>
        {
            TransferManager.Add(transferItem);
            transferItem.Start.Execute().Subscribe();
        });
        
        return transferItemResult;
    }

    private Task<Result<CompositeAction>> GetAction(List<CopiedClipboardEntry> items, IMutableDirectory directory)
    {
        var results = items.Select(entry => ToCopyAction(entry, directory));
        var combine = results.Combine();
        return combine.Map(actions => new CompositeAction(actions.ToArray()));
    }

    private Task<Result<IAction<LongProgress>>> ToCopyAction(CopiedClipboardEntry entry, IMutableDirectory directory)
    {
        var source = FromEntry(entry);
        var destination = directory.Get(entry.Name);

        return source.CombineAndMap(destination, (src, dst) => (IAction<LongProgress>)new CopyFileAction(src, dst));
    }

    private Task<Result<IFile>> FromEntry(CopiedClipboardEntry entry)
    {
        var folder = FileSystems[entry.FileSystemKey].GetDirectory((ZafiroPath)entry.ParentPath).Map(x => x.Value);
        return folder.Bind(x => x.Files().Bind(x => x.TryFirst(x => Equals(x.Name, entry.Name)).ToResult("Not found")));
    }
}