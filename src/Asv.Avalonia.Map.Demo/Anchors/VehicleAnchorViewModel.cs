using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using Asv.Common;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using DynamicData;
using Material.Icons;
using ReactiveUI;
using Brushes = Avalonia.Media.Brushes;

namespace Asv.Avalonia.Map.Demo;

public class VehicleAnchorViewModel : MapAnchorViewModel
{
    private SourceCache<MapAnchorActionViewModel, int> _actionSource;
    private ReadOnlyObservableCollection<MapAnchorActionViewModel> _actions;
    private ReadOnlyObservableCollection<PathFigure> _lines;
    private ReadOnlyObservableCollection<GeoPoint> _path;

    public VehicleAnchorViewModel()
    {
        
        StrokeThickness = 2;
        IsEditable = true;
        ZOrder = 0;
        OffsetX = OffsetXEnum.Center;
        OffsetY = OffsetYEnum.Center;
        IsSelected = false;
        IsVisible = true;
        //Icon = MaterialIconKind.Navigation;
        Size = 34;
        BaseSize = 34;
        IconBrush = Brushes.Crimson;
        Title = "Vehicle";
        Description = "Something:";
        
        PathOpacity = 0.6;
        StrokeThickness = 5;
        Stroke = Brushes.Purple;
        StrokeDashArray = new AvaloniaList<double>(2, 2);
        
        _actionSource = new SourceCache<MapAnchorActionViewModel, int>(_ => _.Order);
        _actionSource.Connect()
            .Bind(out _actions)
            .Subscribe();

        _actionSource.AddOrUpdate(new MapAnchorActionViewModel
        {
            Order = 0,
            Icon = MaterialIconKind.About,
            Title = "About",
            Command = ReactiveCommand.Create(() => { })
        });

        _actionSource.AddOrUpdate(new MapAnchorActionViewModel
        {
            Order = 1,
            Icon = MaterialIconKind.Dog,
            Title = "Dog",
            Command = ReactiveCommand.Create(() => { })
        });
        
        var cache1 = new SourceList<GeoPoint>();
        cache1.Add(new GeoPoint(0, 0, 0));
        cache1.Add(new GeoPoint(0, 0, 0));


        cache1.Connect()
            .Bind(out _path)
            .Subscribe();
        
        SetUpLines();
    }

    private void SetUpLines()
    {
        var cache = new SourceList<PathFigure>();

        var line1 = new PathFigure
        {
            StartPoint = new Point(),
            Segments = new PathSegments()
            {
                new LineSegment()
                {
                    Point = new Point(1000, 100)
                }
            }
        };

        var line2 = new PathFigure
        {
            StartPoint = new Point(0, 0),
            Segments = new PathSegments()
            {
                new LineSegment()
                {
                    Point = new Point(100, 2000)
                }
            }
        };
        
        cache.Add(line1);
        cache.Add(line2);

        cache.Connect()
            .Bind(out _lines)
            .Subscribe();
    }

    public override ReadOnlyObservableCollection<MapAnchorActionViewModel> Actions => _actions;
    public override ReadOnlyObservableCollection<PathFigure> Lines => _lines;
    public override ReadOnlyObservableCollection<GeoPoint> Path => _path;
}