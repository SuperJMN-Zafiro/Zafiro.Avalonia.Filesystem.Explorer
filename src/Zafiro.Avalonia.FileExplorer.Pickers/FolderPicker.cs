using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.ViewModels;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Pickers
{
    public class FolderPicker : IFolderPicker
    {
        private readonly IDialogService dialogService;
        private readonly IFileSystem fileSystem;
        private readonly INotificationService notificationService;

        public FolderPicker(IDialogService dialogService, IFileSystem fileSystem, INotificationService notificationService)
        {
            this.dialogService = dialogService;
            this.fileSystem = fileSystem;
            this.notificationService = notificationService;
        }

        public IObservable<Maybe<IZafiroDirectory>> Pick(string title)
        {
            var folderContentsViewModel = new FolderContentsViewModel(fileSystem, DirectoryListing.GetDirectories,notificationService);
            var fromAsync = Observable
                .FromAsync(() =>
                {
                    var pickAFolder = title;
                    var okTitle = "Select";
                    return dialogService.Prompt(pickAFolder, folderContentsViewModel, okTitle, model => ReactiveCommand.CreateFromObservable(() => Observable.FromAsync(() => fileSystem.GetDirectory(model.Path)).Successes()));
                });

            return fromAsync;
        }
    }
}