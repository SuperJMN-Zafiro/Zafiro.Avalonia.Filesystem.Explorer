using Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class FileItemViewModel : ReactiveObject, IFile
{
    private readonly ISelectionContext selectionContext;
    public IZafiroFile File { get; }

    public FileItemViewModel(IZafiroFile file, IContentOpener fileOpener, ISelectionContext selectionContext, INotificationService notificationService)
    {
        this.selectionContext = selectionContext;
        File = file;
        Open = ReactiveCommand.CreateFromTask(() => fileOpener.Open(file.Contents, file.Path.Name()));
        Open.HandleErrorsWith(notificationService);
        IsBusy = Open.IsExecuting;
    }

    public IObservable<bool> IsBusy { get; }

    public ReactiveCommand<Unit, Result> Open { get; set; }

    public ZafiroPath Path => File.Path;

    public IObservable<byte> GetStream()
    {
        return File.Contents;
    }

    public string Name => Path.Name();

    public long Size { get; }

    [Reactive]
    public bool IsSelected { get; set; }

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy => selectionContext.Copy;
    public ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Paste => selectionContext.Paste;
}