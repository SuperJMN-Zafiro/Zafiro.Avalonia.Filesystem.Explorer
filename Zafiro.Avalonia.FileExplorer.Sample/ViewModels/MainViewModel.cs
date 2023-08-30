using System;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Serilog;
using Zafiro.Avalonia.FileExplorer.ViewModels;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.SeaweedFS;
using Zafiro.FileSystem.SeaweedFS.Filer.Client;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly SeaweedFileSystem fileSystem;
    private readonly ObservableAsPropertyHelper<FolderViewModel> vm;
    private readonly ObservableAsPropertyHelper<DetailsViewModel> details;

    public MainViewModel()
    {
        fileSystem = new SeaweedFileSystem(new SeaweedFSClient(new HttpClient() { BaseAddress = new Uri("http://192.168.1.31:8888") }), Maybe<ILogger>.None);
        var command = ReactiveCommand.CreateFromTask(() => fileSystem.GetDirectory("/"));
        vm = command.Successes().Select(m => new FolderViewModel(m)).ToProperty(this, x => x.FolderViewModel);
        command.Failures().Subscribe(s => { });
        command.Execute().Subscribe();

        var loadDetails = ReactiveCommand.CreateFromTask(() => fileSystem.GetDirectory("/Música"));
        
        details = loadDetails.Successes().Select(directory => new DetailsViewModel(directory)).ToProperty(this, x => x.DetailsViewModel);
        loadDetails.Execute().Subscribe();
    }

    public DetailsViewModel DetailsViewModel => details.Value;

    public FolderViewModel FolderViewModel => vm.Value;
}