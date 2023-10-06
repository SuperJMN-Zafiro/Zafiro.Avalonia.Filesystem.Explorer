using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Clipboard;
using Zafiro.Avalonia.FileExplorer.Explorer;
using Zafiro.Avalonia.FileExplorer.TransferManager;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Pickers
{
    public class FolderPicker : IFolderPicker
    {
        private readonly IDialogService dialogService;
        private readonly IFileSystem fileSystem;
        private readonly INotificationService notificationService;
        private readonly IClipboard clipboard;
        private readonly ITransferManager transferManager;

        public FolderPicker(IDialogService dialogService, IFileSystem fileSystem, INotificationService notificationService, IClipboard clipboard, ITransferManager transferManager)
        {
            this.dialogService = dialogService;
            this.fileSystem = fileSystem;
            this.notificationService = notificationService;
            this.clipboard = clipboard;
            this.transferManager = transferManager;
        }

        public IObservable<Maybe<IZafiroDirectory>> Pick(string title)
        {
            var folderContentsViewModel = new FileSystemExplorer(fileSystem, notificationService, clipboard, transferManager);
            var fromAsync = Observable
                .FromAsync(() =>
                {
                    var pickAFolder = title;
                    var okTitle = "Select";
                    return dialogService.ShowDialog(
                        folderContentsViewModel, 
                        pickAFolder, 
                        model => Observable.FromAsync(() => model.Result), 
                        new OptionConfiguration<FileSystemExplorer, ZafiroPath>("OK", explorer => ReactiveCommand.Create(() => explorer.SetResult(explorer.Address.CurrentDirectory.Path))));
                })
                .SelectMany(path =>
                {
                    return path.Map(zafiroPath => fileSystem.GetDirectory(zafiroPath));
                })
                .Select(maybe => maybe.Where(result => result.IsSuccess).Select(x => x.Value));

            return fromAsync;
        }
    }
}