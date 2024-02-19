using System;
using System.Collections.ObjectModel;
using FluentAvalonia.UI.Controls;

namespace Asv.Avalonia.Map.Demo
{
    public interface IShellMenuItem : IViewModel
    {
        InfoBadge InfoBadge { get; set; }
        IShellMenuItem? Parent { get; set; }
        string Name { get; set; }
        Uri NavigateTo { get; set; }
        string Icon { get; }
        int Order { get; }
        ReadOnlyObservableCollection<IShellMenuItem>? Items { get; }
        bool IsSelected { get; set; }
        bool IsVisible { get; set; }
    }
    
    public interface IShellMenuItem<in TTarget>:IShellMenuItem
    {
        IShellMenuItem Init(TTarget target);
    }
}
