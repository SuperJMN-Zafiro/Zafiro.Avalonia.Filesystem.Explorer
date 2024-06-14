using System.Windows.Input;
using ReactiveUI;
using Zafiro.FileSystem;
using Zafiro.FileSystem.DynamicData;

namespace Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;

public class FileViewModel : IEntry
{
    public IFile File { get; }

    public FileViewModel(IDynamicDirectory directory, IFile file)
    {
        File = file;
        Delete = ReactiveCommand.CreateFromTask(() => directory.DeleteFile(Name));
    }

    public string Name => File.Name;
    public ICommand Delete { get; }
    public string Key => Name;
}