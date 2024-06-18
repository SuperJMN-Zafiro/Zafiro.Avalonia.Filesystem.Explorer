using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;
using Zafiro.FileSystem.DynamicData;
using Zafiro.FileSystem.Mutable.Mutable;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core;

public interface IPathNavigator
{
    void SetAndLoad(ZafiroPath requestedPath);
    ReactiveCommandBase<Unit, Result<IRooted<IMutableDirectory>>> LoadRequestedPath { get; }
    IObservable<Maybe<IRooted<IMutableDirectory>>> Directories { get; }
    Maybe<IRooted<IMutableDirectory>> CurrentDirectory { get; }
}