using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Zafiro.Actions;
using Zafiro.Avalonia.FileExplorer.Core.DirectoryContent;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;
using Zafiro.Reactive;
using IFile = Zafiro.FileSystem.Readonly.IFile;

namespace Zafiro.Avalonia.FileExplorer.Core.Clipboard;

public class ClipboardService : IClipboardService
{
    private const string MimeType = "x-special/zafiro-copied-files";

    public ClipboardService(IClipboard clipboard, ITransferManager transferManager,
        IDictionary<string, IMutableFileSystem> fileSystems)
    {
        Clipboard = clipboard;
        TransferManager = transferManager;
        FileSystems = fileSystems;
        CanPaste = Observable.Timer(TimeSpan.FromSeconds(0.5))
            .Repeat()
            .Select(_ => Observable.FromAsync(() => GetCopiedItems().Map(list => list.Any()).Match(b => b, _ => false)))
            .Concat()
            .ReplayLastActive()
            .ObserveOn(RxApp.MainThreadScheduler);
        
        CanPaste.Subscribe(b => { });
    }

    public IObservable<bool> CanPaste { get; }

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
        var data = GetCopiedItems()
            .Bind(clipboardEntries => Paste(clipboardEntries, destination));

        return data;
    }

    private Task<Result<List<CopiedClipboardEntry>>> GetCopiedItems()
    {
        return Result.Try(() => Clipboard.GetDataAsync(MimeType))
            .EnsureNotNull("Nothing to paste")
            .Map(o => (byte[]?)o!)
            .Map(Decode)
            .Map(s => JsonSerializer.Deserialize<List<CopiedClipboardEntry>>(s)!);
    }

    private static string Decode(byte[] bytes)
    {
        if (OperatingSystem.IsLinux())
        {
            return Encoding.UTF8.GetString(bytes);
        }

        if (OperatingSystem.IsWindows())
        {
            return Encoding.Unicode.GetString(bytes).TrimEnd('\0');    
        }

        throw new NotSupportedException("Can't decode clipboards content");
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
            transferItem.Transfer.Start.Execute().Subscribe();
        });
        
        return transferItemResult;
    }

    private Task<Result<CompositeAction>> GetAction(List<CopiedClipboardEntry> items, IMutableDirectory directory)
    {
        var results = items.Select(entry => ToCopyAction(entry, directory));
        var combine = results.Combine();
        return combine.Map(actions => new CompositeAction(actions.ToArray()));
    }

    private async Task<Result<IAction<LongProgress>>> ToCopyAction(CopiedClipboardEntry entry, IMutableDirectory directory)
    {
        var source = await FromEntry(entry);
        var destination = await directory.MutableFile(entry.Name).Bind(maybe => maybe.ToResult($"Can't find file {entry.Name} in {directory}"));

        return source.CombineAndMap(destination, (src, dst) => (IAction<LongProgress>)new CopyFileAction(src, dst));
    }

    private Task<Result<IFile>> FromEntry(CopiedClipboardEntry entry)
    {
        var folder = FileSystems[entry.FileSystemKey].GetDirectory(entry.ParentPath);
        return folder
            .Bind(x => x.Files().ToTask())
            .Bind(x => x
                .TryFirst(mutableFile => Equals(mutableFile.Name, entry.Name))
                .ToResult("Not found")
                .Map(file => file.AsReadOnly()));
    }
}