using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;
using Zafiro.Avalonia.FileExplorer.TransferManager.Items;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class FileItemViewModel : ReactiveObject, IFile
{
    private readonly ISelectionContext selectionContext;

    public FileItemViewModel(IZafiroFile file, ExplorerContext explorerContext, ISelectionContext selectionContext)
    {
        this.selectionContext = selectionContext;
        File = file;
        Open = ReactiveCommand.CreateFromTask(() => explorerContext.Opener.Open(file.Contents, file.Path.Name()));
        Open.HandleErrorsWith(explorerContext.NotificationService);
        IsBusy = Open.IsExecuting;
    }

    public IZafiroFile File { get; }

    public IObservable<bool> IsBusy { get; }

    public ReactiveCommand<Unit, Result> Open { get; set; }

    public string Name => Path.Name();

    public ZafiroPath Path => File.Path;

    public IObservable<byte> GetStream() => File.Contents;

    public long Size { get; }

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy => selectionContext.Copy;
    public ReactiveCommand<Unit, IList<ITransferItem>> Paste => selectionContext.Paste;
    [Reactive] public bool IsSelected { get; set; }
}