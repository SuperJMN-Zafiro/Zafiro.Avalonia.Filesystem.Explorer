using System.IO.Abstractions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Notifications;
using Zafiro.FileSystem.Local;

namespace AvaloniaApplication1;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
        
        this.Connect(
            () => new MainView(), 
            mv =>
            {
                var topLevel = TopLevel.GetTopLevel(mv)!;
                var notificationService = new NotificationService(new WindowNotificationManager(topLevel));
                return new MainViewModel(new DotNetFileSystem(new FileSystem()), notificationService, new DesktopDialog(this), topLevel.Clipboard!);
            }, () => new MainWindow());
    }
}