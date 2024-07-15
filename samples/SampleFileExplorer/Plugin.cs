using Zafiro.Avalonia.FileExplorer.Core.Clipboard;
using Zafiro.FileSystem.Mutable;

namespace SampleFileExplorer;

public record Plugin(string Identifier, string Name, IMutableFileSystem FileSystem) : IPlugin;