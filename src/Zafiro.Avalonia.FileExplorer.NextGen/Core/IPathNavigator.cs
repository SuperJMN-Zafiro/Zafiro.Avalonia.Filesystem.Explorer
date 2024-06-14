using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.FileSystem;
using Zafiro.FileSystem.DynamicData;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core;

public interface IPathNavigator
{
    void SetAndLoad(ZafiroPath requestedPath);
    ReactiveCommandBase<Unit, Result<IRooted<IDynamicDirectory>>> LoadRequestedPath { get; }
    IObservable<Maybe<IRooted<IDynamicDirectory>>> CurrentDirectory { get; }
}