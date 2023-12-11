using System;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IFileSystemExplorer
{
    ITransferManager TransferManager { get; }
    IToolBar ToolBar { get; }
    IAddress Address { get; }
    DirectoryContentsViewModel Details { get; }
    IClipboard Clipboard { get; }
    IObservable<Maybe<IZafiroDirectory>> CurrentDirectory { get; }
    void GoTo(ZafiroPath path);
}