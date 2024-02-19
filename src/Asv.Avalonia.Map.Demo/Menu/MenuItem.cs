using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using FluentAvalonia.UI.Controls;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.Map.Demo;

public class MenuItem: ViewModelBase, IShellMenuItem
{
    public MenuItem(Uri uri):base(uri)
    {
        
    }
    public InfoBadge InfoBadge { get; set; }
    public IShellMenuItem? Parent { get; set; }
    [Reactive]
    public string Name { get; set; }
    [Reactive]
     public Uri NavigateTo { get; set; }
    [Reactive]
    public string Icon { get; set; }
    public int Order { get; }
    public ReadOnlyObservableCollection<IShellMenuItem>? Items { get; }
    public bool IsSelected { get; set; }
    public bool IsVisible { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;
    public void RaisePropertyChanging(PropertyChangingEventArgs args)
    {
      
    }

    public void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
       
    }

    public void Dispose()
    {
      
    }

    public Uri Id { get; }
}
