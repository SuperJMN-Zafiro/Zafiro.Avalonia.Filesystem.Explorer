using System.IO;
using CSharpFunctionalExtensions;
using Serilog;
using Zafiro.Avalonia.FileExplorer.ViewModels;
using Zafiro.FileSystem.Local;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class MainViewModel : ViewModelBase
{
    public FolderViewModel FolderViewModel { get; } = new(new LocalDirectory(new DirectoryInfo("D:\\"), Maybe<ILogger>.None));
}
