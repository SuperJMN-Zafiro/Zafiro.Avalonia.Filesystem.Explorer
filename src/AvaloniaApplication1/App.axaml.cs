using System;
using System.IO.Abstractions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Notifications;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Local.Mutable;

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
                var notificationService = new NotificationService(new WindowNotificationManager(TopLevel.GetTopLevel(mv)));
                return new MainViewModel(new DotNetFileSystem(new FileSystem()), notificationService, new DesktopDialog(this));
            }, () => new MainWindow());
    }
}