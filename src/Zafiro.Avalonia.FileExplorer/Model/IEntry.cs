using System;
using System.ComponentModel;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IEntry : INotifyPropertyChanged
{
    public ZafiroPath Path { get; }
    public bool IsSelected { get; set; }
}