using Avalonia.Media;
using Material.Icons;

namespace Asv.Avalonia.Map.Demo;

public class AltimeterAnchor: MapAnchorViewModel
{
    public AltimeterAnchor()
    {
        Size = 48;
        BaseSize = 48;
        OffsetX = OffsetXEnum.Center;
        OffsetY = OffsetYEnum.Bottom;
        StrokeThickness = 1;
        IconBrush = Brushes.Blue;
        Stroke = Brushes.CornflowerBlue;
        IsVisible = false;
        Icon = MaterialIconKind.Altimeter;
        IsEditable = true;
        Title = "Altimeter";
    }
}