using System.Reactive.Linq;
using System.Reactive.Subjects;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer.NextGen.Core.DirectoryContent;

public class DirectoryViewModel: ReactiveObject, IDirectoryItem
{
    private readonly Subject<Unit> deleteSubject = new();
    public IRooted<IMutableDirectory> Parent { get; }
    public IMutableDirectory Directory { get; }
    public ExplorerContext Context { get; }

    public DirectoryViewModel(IRooted<IMutableDirectory> parent, IMutableDirectory directory, ExplorerContext context)
    {
        Parent = parent;
        Directory = directory;
        Context = context;
        Navigate = ReactiveCommand.Create(() =>
        {
            context.PathNavigator.SetAndLoad(parent.Path.Combine(directory.Name));
        });
        
        Delete = ReactiveCommand.CreateFromTask(() =>
        {
            return directory.Delete().Tap(() => deleteSubject.OnNext(Unit.Default));
        });

        Delete.HandleErrorsWith(context.NotificationService);
    }

    public ReactiveCommand<Unit, Result> Delete { get; set; }

    public ReactiveCommand<Unit,Unit> Navigate { get; }

    public string Name => Directory.Name;
    public string Key => Directory.Name + "/";
    public IObservable<Unit> Deleted => deleteSubject.AsObservable();
}