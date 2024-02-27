using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Asv.Common;
using Avalonia.Collections;
using Avalonia.Media;
using DynamicData;

namespace Asv.Avalonia.Map.Demo;

public class RulerPolygon : MapAnchorViewModel
{
    private readonly ReadOnlyObservableCollection<GeoPoint> _path;

    public RulerPolygon(Ruler ruler)
    {
        Ruler = new RxValue<Ruler>();

        ZOrder = 1000;
        OffsetX = 0;
        OffsetY = 0;
        PathOpacity = 0.6;
        StrokeThickness = 5;
        Stroke = Brushes.Purple;
        IsVisible = false;
        StrokeDashArray = new AvaloniaList<double>(2, 2);

        var cache = new SourceList<GeoPoint>();
        cache.Add(new GeoPoint(0, 0, 0));
        cache.Add(new GeoPoint(0, 0, 0));

        ruler.IsVisible.Where(_ => _.HasValue).Subscribe(_ => IsVisible = _.Value);
        ruler.Start.Where(_ => _.HasValue).Subscribe(_ => cache.ReplaceAt(0, _.Value));
        ruler.Stop.Where(_ => _.HasValue).Subscribe(_ => cache.ReplaceAt(1, _.Value));

        cache.Connect()
            .Bind(out _path)
            .Subscribe();

        Ruler.OnNext(ruler);
    }

    public override ReadOnlyObservableCollection<GeoPoint> Path => _path;

    public IRxEditableValue<Ruler> Ruler { get; }
}