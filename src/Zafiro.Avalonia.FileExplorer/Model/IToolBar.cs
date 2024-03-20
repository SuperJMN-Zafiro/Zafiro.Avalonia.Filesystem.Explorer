namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IToolBar
{
    ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Delete { get; set; }
    ReactiveCommand<Unit, IList<Result<IAction<LongProgress>>>> Paste { get; }
    ReactiveCommand<Unit, List<IClipboardItem>> Copy { get; set; }
}