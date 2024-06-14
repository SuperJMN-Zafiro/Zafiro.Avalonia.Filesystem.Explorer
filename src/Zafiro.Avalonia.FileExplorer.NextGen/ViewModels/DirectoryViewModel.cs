using Zafiro.FileSystem.DynamicData;

namespace Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;

public class DirectoryViewModel: IEntry
{
    public IDynamicDirectory Parent { get; }
    public IDynamicDirectory Directory { get; }

    public DirectoryViewModel(IDynamicDirectory parent, IDynamicDirectory directory)
    {
        Parent = parent;
        Directory = directory;
    }

    public string Name => Directory.Name;
    public string Key => Directory.Name + "/";
}