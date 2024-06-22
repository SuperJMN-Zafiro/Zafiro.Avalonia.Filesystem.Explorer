namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public interface IHistory<T>
{
    ReactiveCommand<Unit, Unit> GoBack { get; }
    public Maybe<T> CurrentFolder { get; set; }
    public Maybe<T> PreviousFolder { get; }
}