using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IHistory<T>
{
    ReactiveCommand<Unit, Unit> GoBack { get; }
    T CurrentFolder { get; set; }
    Maybe<T> PreviousFolder { get; }
}