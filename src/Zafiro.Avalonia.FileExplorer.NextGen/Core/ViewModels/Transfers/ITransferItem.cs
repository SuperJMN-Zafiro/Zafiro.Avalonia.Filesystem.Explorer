namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels.Transfers;

public interface ITransferItem
{
    string Key { get; }
    Task Start(CancellationToken cancellationToken);
}