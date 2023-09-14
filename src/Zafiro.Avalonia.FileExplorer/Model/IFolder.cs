using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DynamicData;
using Zafiro.FileSystem;

namespace Zafiro.Avalonia.FileExplorer.Model;

public interface IFolder : IEntry
{
    ZafiroPath Path { get; }
    ReadOnlyObservableCollection<IEntry> Children { get; }
    //Task<Result> DeleteFile(string name);
    Task<Result<IEntry>> Add(string name, Stream contents, CancellationToken cancellationToken);
    //Task<Result> CreateFolder(string name);
    //Task<Result> DeleteFolder(string name);
}