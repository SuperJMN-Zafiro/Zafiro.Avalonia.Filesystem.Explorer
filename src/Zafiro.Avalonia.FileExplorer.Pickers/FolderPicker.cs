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
        private readonly Func<IFileSystem> fileSystemFactory;

        public FolderPicker(IDialogService dialogService, Func<IFileSystem> fileSystemFactory)
        {
            this.dialogService = dialogService;
            this.fileSystemFactory = fileSystemFactory;
        }

        public IObservable<Maybe<IZafiroDirectory>> Pick(string title)
        {
            var fileSystem = fileSystemFactory();
            var folderContentsViewModel = new FolderContentsViewModel(fileSystem, DirectoryListing.GetDirectories);
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