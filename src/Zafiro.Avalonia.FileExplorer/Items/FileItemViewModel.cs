using System;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Items;

public class FileItemViewModel : ReactiveObject, IFile
{
    public IZafiroFile File { get; }

    public FileItemViewModel(IZafiroFile file, ISystemOpen fileOpener)
    {
        File = file;
        Open = ReactiveCommand.CreateFromTask(() => fileOpener.Open(file.Contents, file.Path.Name()));
        IsBusy = Open.IsExecuting;
    }

    public IObservable<bool> IsBusy { get; }

    public ReactiveCommand<Unit, Result> Open { get; set; }

    public ZafiroPath Path => File.Path;

    public IObservable<byte> GetStream()
    {
        return File.Contents;
    }

    public string Name => Path.Name();

    public long Size { get; }

    [Reactive]
    public bool IsSelected { get; set; }

}