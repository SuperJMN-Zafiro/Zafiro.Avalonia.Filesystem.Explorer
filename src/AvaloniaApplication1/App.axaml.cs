using System.Collections.Generic;
using System.IO.Abstractions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.Clipboard;
using Zafiro.Avalonia.FileExplorer.NextGen.Core.Transfers;
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

        ConnectRealApp();
        //ConnectTest();
    }

    private void ConnectRealApp()
    {
        this.Connect(
            () => new MainView(),
            mv =>
            {
                var topLevel = TopLevel.GetTopLevel(mv)!;
                var notificationService = new NotificationService(new WindowNotificationManager(topLevel));
                var dotNetFileSystem = new DotNetMutableFileSystem(new FileSystem());
                var dialogService = new DesktopDialog(this);
                ITransferManager transferManager = new TransferManager();
                var clipboardService = new ClipboardService(topLevel.Clipboard!, transferManager, new Dictionary<string, Zafiro.FileSystem.Mutable.IMutableFileSystem>()
                {
                    ["local"] = new DotNetMutableFileSystem(new FileSystem()),
                });
                return new MainViewModel(dotNetFileSystem, notificationService, dialogService, clipboardService, transferManager);
            }, () => new MainWindow());
    }

    private void ConnectTest()
    {
        this.Connect(
            () => new TestView(), 
            mv =>
            {
                var topLevel = TopLevel.GetTopLevel(mv)!;
                var notificationService = new NotificationService(new WindowNotificationManager(topLevel));
                var dotNetFileSystem = new DotNetMutableFileSystem(new FileSystem());
                var dialogService = new DesktopDialog(this);
                ITransferManager transferManager = new TransferManager();
                var clipboardService = new ClipboardService(topLevel.Clipboard!, transferManager, new Dictionary<string, Zafiro.FileSystem.Mutable.IMutableFileSystem>()
                {
                    ["local"] = (Zafiro.FileSystem.Mutable.IMutableFileSystem)new DotNetMutableFileSystem(new FileSystem()),
                });
                return new TestViewModel(dotNetFileSystem, notificationService);
            }, () => new MainWindow());
    }
}