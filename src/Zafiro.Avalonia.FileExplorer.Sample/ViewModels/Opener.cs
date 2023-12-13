using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.FileExplorer.Items;
using Zafiro.IO;

namespace Zafiro.Avalonia.FileExplorer.Sample.ViewModels;

public class Opener : ISystemOpen
{
    public Task<Result> Open(IObservable<byte> fileContents, string fileName)
    {
        return Result.Try(async () =>
        {
            var tempFileName = Path.Combine(Path.GetTempPath(), fileName);
            await using (var fileStream = File.Create(tempFileName))
            {
                await fileContents.DumpTo(fileStream);
            }

            Process.Start(new ProcessStartInfo()
            {
                FileName = tempFileName,
                UseShellExecute = true,
            });
        });
    }
}