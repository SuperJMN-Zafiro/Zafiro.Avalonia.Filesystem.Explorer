using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Mutable;
using Zafiro.UI;

namespace AvaloniaApplication1.ViewModels;

public class TestViewModel
{
    public TestViewModel(IMutableFileSystem fs, INotificationService notificationService)
    {
        Copy = ReactiveCommand.CreateFromTask(() =>
        {
            var file1 = fs.GetFile(
                "home/jmn/Descargas/15 Clasicos de Disney (Parte 2 de 3) (DVDRip) (EliteTorrent.net)/La Dama y el Vagabundo (1955).divx");
            var file2 = fs.GetFile("home/jmn/Escritorio/Copied.divx");

            return file1.CombineAndBind(file2, (a, b) =>
            {
                return a.Value.GetContents().Bind(data => b.Value.SetContents(data, scheduler: TaskPoolScheduler.Default));
                //return a.Value.GetContents().Bind(_ => Result.Success());
            });
        });

        Copy.Subscribe(result => notificationService.Show(result.ToString()));
    }

    public ReactiveCommand<Unit, Result> Copy { get; set; }
}