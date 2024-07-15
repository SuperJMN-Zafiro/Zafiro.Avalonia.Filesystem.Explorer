namespace Zafiro.Avalonia.FileExplorer.Core.Clipboard;

public interface IPlugin
{
    public IMutableFileSystem FileSystem { get; }
    public string Name { get; }
    public string Identifier { get; }
}