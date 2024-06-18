using System.Windows.Input;
using Zafiro.FileSystem.Core;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

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