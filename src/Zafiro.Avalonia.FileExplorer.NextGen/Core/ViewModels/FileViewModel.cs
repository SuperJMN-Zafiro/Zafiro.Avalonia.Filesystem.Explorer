using System.Windows.Input;
using ReactiveUI;
using Zafiro.FileSystem;
using Zafiro.FileSystem.DynamicData;
using Zafiro.FileSystem.Mutable.Mutable;

namespace Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;

public class FileViewModel : IEntry
{
    public IMutableFile File { get; }

    public FileViewModel(IRooted<IMutableDirectory> directory, IMutableFile file)
    {
        File = file;
        //Delete = ReactiveCommand.CreateFromTask(() => directory(Name));
    }

    public string Name => File.Name;
    public ICommand Delete { get; }
    public string Key => Name;
}