using System.ComponentModel;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IEntry : INotifyPropertyChanged
{
    public ZafiroPath Path { get; }
    public bool IsSelected { get; set; }
}