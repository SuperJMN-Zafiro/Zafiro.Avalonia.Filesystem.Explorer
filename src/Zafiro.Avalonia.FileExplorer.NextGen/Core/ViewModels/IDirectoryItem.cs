using System.Windows.Input;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public interface IDirectoryItem : INamed
{
    public string Key { get;  }
    IObservable<Unit> Deleted { get; }
    ReactiveCommand<Unit, Result> Delete { get; }
}