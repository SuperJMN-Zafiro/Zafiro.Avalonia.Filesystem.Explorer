using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core;

public interface IHistory<T>
{
    ReactiveCommand<Unit, Unit> GoBack { get; }
    public Maybe<T> CurrentFolder { get; set; }
    public Maybe<T> PreviousFolder { get; }
}