// See https://aka.ms/new-console-template for more information

using DynamicData;
using Zafiro.FileSystem.DynamicData;
using Zafiro.FileSystem.Local.Mutable;

Console.WriteLine("Hello World!");
var fs = new System.IO.Abstractions.FileSystem();
var directoryInfo = fs.DirectoryInfo.New("/home/jmn/Escritorio");
var directory = new LocalDynamicDirectory(directoryInfo);
directory.AllFiles().Subscribe(Write);
Console.ReadLine();

static void Write<T, Q>(IChangeSet<T,Q> changeSets)
{
    Console.WriteLine();
    foreach (var change in changeSets)
    {
        
        Console.WriteLine(change);
    }
}