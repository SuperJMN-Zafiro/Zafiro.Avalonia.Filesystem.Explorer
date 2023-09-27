using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zafiro.UI;

namespace Zafiro.Avalonia.FileExplorer;

// TODO: Move to Zafiro.UI
public class StoppableCommandFactory
{
    public static IStoppableCommand CreateFromTask<TOut>(Func<CancellationToken, Task<TOut>> task, IObservable<bool> canExecute)
    {
        return new StoppableCommand<Unit, TOut>(_ => Observable.FromAsync(task), canExecute);
    }
}
