using System.Reactive;
using Zafiro.FileSystem.Core;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public interface IDirectoryItem : INamed
{
    public string Key { get;  }
    IObservable<Unit> Deleted { get; }
}