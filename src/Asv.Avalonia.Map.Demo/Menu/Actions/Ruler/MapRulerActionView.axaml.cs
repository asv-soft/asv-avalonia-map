
using Asv.Avalonia.Map.Demo.Menu.Actions.Ruler;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Asv.Drones.Gui.Core;

public partial class MapRulerActionView : ReactiveUserControl<MapRulerActionViewModel>
{
    public MapRulerActionView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}