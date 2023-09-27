using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;

using Zafiro.Avalonia.FileExplorer.Sample.ViewModels;
using Zafiro.Avalonia.FileExplorer.Sample.Views;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Notifications;

namespace Zafiro.Avalonia.FileExplorer.Sample;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        this.Connect(() => new MainView(), mv => new MainViewModel(new NotificationService(new WindowNotificationManager(TopLevel.GetTopLevel(mv)))));

        base.OnFrameworkInitializationCompleted();
    }
}
