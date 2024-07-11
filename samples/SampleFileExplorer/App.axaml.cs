using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using SampleFileExplorer.ViewModels;
using SampleFileExplorer.Views;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.FileExplorer.Core.Clipboard;
using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.Avalonia.Mixins;
using Zafiro.Avalonia.Notifications;
using Zafiro.FileSystem.Local.Mutable;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace SampleFileExplorer;

public class App : Application
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
                var notificationService = new NotificationService(new WindowNotificationManager(topLevel) { Position = NotificationPosition.BottomRight });
                var fs = new Zafiro.FileSystem.SeaweedFS.Filesystem(new SeaweedFSClient(new HttpClient(){ BaseAddress = new Uri("http://192.168.1.29:8888")}));
                var dialogService = new DesktopDialog(this);
                ITransferManager transferManager = new TransferManager();
                var clipboardService = new ClipboardService(topLevel.Clipboard!, transferManager, new Dictionary<string, Zafiro.FileSystem.Mutable.IMutableFileSystem>()
                {
                    ["local"] = new DotNetMutableFileSystem(new FileSystem()),
                });
                return new MainViewModel(fs, notificationService, dialogService, clipboardService, transferManager);
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
                var fs = new Zafiro.FileSystem.SeaweedFS.Filesystem(new SeaweedFSClient(new HttpClient(){ BaseAddress = new Uri("http://192.168.1.29:8888")}));
                return new TestViewModel(fs, notificationService);
            }, () => new MainWindow());
    }
}