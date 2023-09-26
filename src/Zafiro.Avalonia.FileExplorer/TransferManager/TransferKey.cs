using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.TransferManager;

public record TransferKey(ZafiroPath Source, ZafiroPath Destination);