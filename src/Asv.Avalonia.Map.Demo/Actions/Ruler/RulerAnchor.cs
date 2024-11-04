using System;
using System.Reactive.Linq;
using Asv.Avalonia.Map;
using Asv.Avalonia.Map.Demo;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI;

namespace Asv.Avalonia.GMap.Demo;

public enum RulerPosition
{
    Start,
    Stop,
}

public class RulerAnchor : MapAnchorViewModel
{
    public RulerAnchor(string id, Ruler ruler, RulerPosition rulerPosition)
    {
        Size = 48;
        BaseSize = 48;
        OffsetX = OffsetXEnum.Center;
        OffsetY = OffsetYEnum.Bottom;
        StrokeThickness = 1;
        IconBrush = Brushes.Indigo;
        Stroke = Brushes.White;
        IsVisible = false;
        Icon = MaterialIconKind.MapMarker;
        IsEditable = true;

        ruler.IsVisible.Where(_ => _.HasValue).Subscribe(_ => IsVisible = _.Value);

        if (rulerPosition == RulerPosition.Stop)
            ruler.Distance.Subscribe(_ => Title = $"{_:F1} m");

        var isLocationInternalChanged = false;
        var point = rulerPosition == RulerPosition.Start ? ruler.Start : ruler.Stop;

        point
            .Where(_ => _.HasValue)
            .Subscribe(_ =>
            {
                isLocationInternalChanged = true;
                Location = _.Value;
                isLocationInternalChanged = false;
            });

        this.WhenAnyValue(_ => _.Location)
            .Where(_ => !isLocationInternalChanged)
            .Subscribe(_ => point.OnNext(_));
    }
}
