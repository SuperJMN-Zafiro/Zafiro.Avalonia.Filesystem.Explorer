namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IHistory<T>
{
    ReactiveCommand<Unit, Unit> GoBack { get; }
    public Maybe<T> CurrentFolder { get; set; }
    public Maybe<T> PreviousFolder { get; }
}