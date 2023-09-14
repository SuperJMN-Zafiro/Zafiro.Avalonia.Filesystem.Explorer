using System;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IEntry
{
    public ZafiroPath Path { get; }
    public IObservable<bool> IsSelected { get; }
}