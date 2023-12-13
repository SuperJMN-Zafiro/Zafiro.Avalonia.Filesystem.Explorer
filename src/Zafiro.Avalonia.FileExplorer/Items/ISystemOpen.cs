using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.FileExplorer.Items;

public interface ISystemOpen
{
    Task<Result> Open(IObservable<byte> fileContents, string fileName);
}