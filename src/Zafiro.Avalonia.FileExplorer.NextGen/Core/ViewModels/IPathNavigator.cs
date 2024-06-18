using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public interface IPathNavigator
{
    void SetAndLoad(ZafiroPath requestedPath);
    ReactiveCommandBase<Unit, Result<IRooted<IMutableDirectory>>> LoadRequestedPath { get; }
    IObservable<Maybe<IRooted<IMutableDirectory>>> Directories { get; }
    Maybe<IRooted<IMutableDirectory>> CurrentDirectory { get; }
}