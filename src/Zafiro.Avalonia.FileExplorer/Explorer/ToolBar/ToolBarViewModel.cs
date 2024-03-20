namespace Zafiro.Avalonia.FileExplorer.Explorer.ToolBar;

public class ToolBarViewModel : IToolBar
{
    private readonly ISelectionContext selectionContext;

    public ToolBarViewModel(ISelectionContext selectionContext)
    {
        this.selectionContext = selectionContext;
        Delete = selectionContext.Delete;
        Copy = selectionContext.Copy;
        Paste = selectionContext.Paste;
        IsPasting = selectionContext.IsPasting;
    }

    public IObservable<bool> IsPasting { get; }

    public bool IsSelectionEnabled
    {
        get => selectionContext.IsTouchFriendlySelectionEnabled;
        set => selectionContext.IsTouchFriendlySelectionEnabled = value;
    }

    public ReactiveCommand<Unit, IList<IAction<LongProgress>>> Delete { get; set; }

    public ReactiveCommand<Unit, IAction<LongProgress>> Paste { get; }

    public ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; set; }
}