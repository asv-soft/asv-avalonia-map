using System;
using Asv.Common;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI;

namespace Asv.Avalonia.Map.Demo;

public class AltimeterAnchor : MapAnchorViewModel
{
    public HeightProviderBase HeightProvider;
    public AltimeterAnchor()
    {
        HeightProvider = new AsterHeightProvider();
        Size = 48;
        BaseSize = 48;
        OffsetX = OffsetXEnum.Center;
        OffsetY = OffsetYEnum.Center;
        StrokeThickness = 1;
        IconBrush = Brushes.Red;
        Stroke = Brushes.Pink;
        IsVisible = false;
        Icon = MaterialIconKind.Plus;
        IsEditable = true;
        Title = $"{Location.Altitude} m.";
        Location = HeightProvider.GetPointAltitude(Location).Result;
        this.WhenAnyValue(_ => _.Location, _=>_.IsDragged).Subscribe(_ =>
        {
            if (IsDragged) return;
            var locationWithAlt = HeightProvider.GetPointAltitude(Location).Result;
            Location = new GeoPoint(locationWithAlt.Latitude, locationWithAlt.Longitude, locationWithAlt.Altitude);
            Title = $"{Location.Altitude} m.";
        });
    }
    
}