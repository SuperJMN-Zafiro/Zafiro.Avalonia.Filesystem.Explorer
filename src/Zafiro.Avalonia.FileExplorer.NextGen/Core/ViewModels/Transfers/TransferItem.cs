using Zafiro.FileSystem.Core;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

internal class TransferItem : ITransferItem
{
    public TransferItem(string name, ZafiroPath path, ItemType itemType)
    {
        Path = path;
        Name = name;
        ItemType = itemType;
    }

    public string Key => Path.Combine(Name) + ItemType; 
    public ZafiroPath Path { get; }
    public string Name { get; }

    public ItemType ItemType { get; }
}