using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;
using Zafiro.Avalonia.FileExplorer.ViewModels;

namespace Zafiro.Avalonia.FileExplorer.Sample;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        var name = data.GetType().AssemblyQualifiedName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control) Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}