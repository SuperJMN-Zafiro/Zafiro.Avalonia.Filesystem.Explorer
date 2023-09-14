﻿using System;
using System.Net.Http;
using System.Reactive;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Serilog;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.FileExplorer.Model;
using Zafiro.Avalonia.FileExplorer.Pickers;
using Zafiro.Avalonia.FileExplorer.ViewsModes.FolderContents;
using Zafiro.FileSystem;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;
using Zafiro.Avalonia.FileExplorer.Items;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class MainViewModel : ReactiveObject
{
    private readonly SeaweedFileSystem fileSystem;
    private readonly ObservableAsPropertyHelper<FolderItemViewModel> vm;
    private readonly ObservableAsPropertyHelper<DetailsViewModel> details;

    public MainViewModel()
    {
        fileSystem = new SeaweedFileSystem(new SeaweedFSClient(new HttpClient() { BaseAddress = new Uri("http://192.168.1.31:8888") }), Maybe<ILogger>.None);
        var notificationService = new NotificationDialog(new DesktopDialogService(Maybe<Action<ConfigureWindowContext>>.None));
        Contents = new FolderContentsViewModel(fileSystem, DirectoryListing.GetAll, notificationService);
        var picker = new FolderPicker(new DesktopDialogService(Maybe<Action<ConfigureWindowContext>>.None), fileSystem, notificationService);
        //var command = ReactiveCommand.CreateFromTask(() => fileSystem.GetDirectory("/"));
        //vm = command.Successes().Select(m => new FolderViewModel(m)).ToProperty(this, x => x.FolderViewModel);
        //command.Failures().Subscribe(s => { });
        //command.Execute().Subscribe();

        //var loadDetails = ReactiveCommand.CreateFromTask(() => fileSystem.GetDirectory("/Música"));

        //details = loadDetails.Successes().Select(directory => new DetailsViewModel(directory)).ToProperty(this, x => x.DetailsViewModel);
        //loadDetails.Execute().Subscribe();
        Pick = ReactiveCommand.CreateFromObservable(() => picker.Pick("Pick a folder"));
        Pick.Subscribe(maybe => { });
    }

    public ReactiveCommand<Unit, Maybe<IZafiroDirectory>> Pick { get; set; }

    public FolderContentsViewModel Contents { get; }
    
    public DetailsViewModel DetailsViewModel => details.Value;

    public FolderItemViewModel FolderViewModel => vm.Value;
}