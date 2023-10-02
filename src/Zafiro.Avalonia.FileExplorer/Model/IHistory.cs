using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IHistory<T>
{
    ReactiveCommand<Unit, Unit> GoBack { get; }
    public T CurrentFolder { get; set; }
    public Maybe<T> PreviousFolder { get; }
}