namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IPathNavigator
{
    void SetAndLoad(ZafiroPath requestedPath);
    ReactiveCommandBase<Unit, Result<IZafiroDirectory>> LoadRequestedPath { get; }
    IObservable<Maybe<IZafiroDirectory>> CurrentDirectory { get; }
}