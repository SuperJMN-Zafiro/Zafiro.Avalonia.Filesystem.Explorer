using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class NotificationDialog : INotificationService
{
    private readonly IDialogService dialogService;

    public NotificationDialog(IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    public Task Show(string message, Maybe<string> title)
    {
        return dialogService.ShowMessage("Dismiss", title.GetValueOrDefault(), message);
    }
}