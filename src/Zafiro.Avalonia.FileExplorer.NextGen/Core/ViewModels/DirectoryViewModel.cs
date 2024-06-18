using System.Reactive;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.NextGen.Core;
using Zafiro.FileSystem;
using Zafiro.FileSystem.Mutable;

namespace Zafiro.Avalonia.FileExplorer.NextGen.ViewModels;

public class DirectoryViewModel: ReactiveObject, IEntry
{
    public IRooted<IMutableDirectory> Parent { get; }
    public IMutableDirectory Directory { get; }
    public ExplorerContext Context { get; }

    public DirectoryViewModel(IRooted<IMutableDirectory> parent, IMutableDirectory directory, ExplorerContext context)
    {
        Parent = parent;
        Directory = directory;
        Context = context;
        Navigate = ReactiveCommand.Create(() =>
        {
            context.PathNavigator.SetAndLoad(parent.Path.Combine(directory.Name));
        });
    }

    public ReactiveCommand<Unit,Unit> Navigate { get; }

    public string Name => Directory.Name;
    public string Key => Directory.Name + "/";
}