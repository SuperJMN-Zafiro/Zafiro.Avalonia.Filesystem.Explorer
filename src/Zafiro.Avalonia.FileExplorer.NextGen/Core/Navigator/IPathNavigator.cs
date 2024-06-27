namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.Navigator;

public interface IPathNavigator
{
    void SetAndLoad(ZafiroPath requestedPath);
    ReactiveCommandBase<Unit, Result<IRooted<IMutableDirectory>>> LoadRequestedPath { get; }
    IObservable<Maybe<IRooted<IMutableDirectory>>> Directories { get; }
    Maybe<IRooted<IMutableDirectory>> CurrentDirectory { get; }
}