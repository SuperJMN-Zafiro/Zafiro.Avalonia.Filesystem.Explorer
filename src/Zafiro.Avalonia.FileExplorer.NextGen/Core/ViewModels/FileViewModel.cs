using System.Reactive.Linq;
using System.Windows.Input;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.ViewModels;

public class FileViewModel : IDirectoryItem
{
    public IMutableFile File { get; }

    public FileViewModel(IMutableFile file)
    {
        File = file;
        Delete = ReactiveCommand.CreateFromTask(file.Delete);
    }

    public string Name => File.Name;
    public ReactiveCommand<Unit, Result> Delete { get; }
    public string Key => Name;
    public IObservable<Unit> Deleted => Observable.Never(Unit.Default);
}