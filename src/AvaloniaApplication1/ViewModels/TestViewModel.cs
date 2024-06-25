using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.FileSystem.Actions;
using Zafiro.FileSystem.Mutable;
using Zafiro.UI;

namespace AvaloniaApplication1.ViewModels;

public class TestViewModel
{
    private readonly BehaviorSubject<double> progressSubject = new BehaviorSubject<double>(0d);

    public TestViewModel(IMutableFileSystem fs, INotificationService notificationService)
    {
        Copy = ReactiveCommand.CreateFromTask(() =>
        {
            var file1 = fs.GetFile(
                "D:/Películas/Vaiana (microHD) (EliteTorrent.net).mkv");
            var file2 = fs.GetFile("c:/users/jmn/Desktop/Copied.divx");

            return file1.CombineAndBind(file2, (a, b) => a.Value.GetContents()
                .Map(data => new CopyFileAction(data, b.Value))
                .Bind(async fileAction =>
                {
                    using (fileAction.Progress.Select(x => x.Value).Subscribe(progressSubject))
                    {
                        return await fileAction.Execute().ConfigureAwait(false);
                    }
                }));
        });

        Copy.Subscribe(result => notificationService.Show(result.ToString()));
    }

    public IObservable<double> Progress => progressSubject.AsObservable();

    public ReactiveCommand<Unit, Result> Copy { get; set; }
}