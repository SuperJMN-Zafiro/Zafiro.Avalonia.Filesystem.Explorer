using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IEntryFactory
{
    Task<Result<IEnumerable<IEntry>>> Get(IZafiroDirectory directory);
}