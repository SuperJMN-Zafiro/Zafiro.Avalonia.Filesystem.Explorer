using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Core;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

internal class TransferItem : ITransferItem
{
    public TransferItem(string name, ZafiroPath path, ItemType itemType, IFileAction action)
    {
        Path = path;
        Name = name;
        ItemType = itemType;
        Action = action;
    }

    public string Key => Path.Combine(Name) + ItemType; 
    public ZafiroPath Path { get; }
    public string Name { get; }

    public ItemType ItemType { get; }
    public IFileAction Action { get; }
}