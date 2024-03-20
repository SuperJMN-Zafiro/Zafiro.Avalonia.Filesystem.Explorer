namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IToolBar
{
    ReactiveCommand<Unit, IList<IAction<LongProgress>>> Delete { get; set; }
    ReactiveCommand<Unit, IAction<LongProgress>> Paste { get; }
    ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; set; }
}