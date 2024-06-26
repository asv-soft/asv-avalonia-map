using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using Material.Icons;
using ReactiveUI;
using Brushes = Avalonia.Media.Brushes;

namespace Asv.Avalonia.Map.Demo;

public class VehicleAnchorViewModel : MapAnchorViewModel
{
    private SourceCache<MapAnchorActionViewModel, int> _actionSource;
    private ReadOnlyObservableCollection<MapAnchorActionViewModel> _actions;

    public VehicleAnchorViewModel()
    {
        Stroke = Brushes.Aqua;
        StrokeThickness = 2;
        IsEditable = true;
        ZOrder = 0;
        OffsetX = OffsetXEnum.Center;
        OffsetY = OffsetYEnum.Center;
        IsSelected = false;
        IsVisible = true;
        Icon = MaterialIconKind.Navigation;
        Size = 34;
        BaseSize = 34;
        IconBrush = Brushes.Crimson;
        Title = "Vehicle";
        Description = "Something:";
        
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

        /*
        _actionSource.AddOrUpdate(new MapAnchorActionViewModel
        {
            Order = 1,
            Icon = MaterialIconKind.Dog,
            Title = "Dog",
            Command = ReactiveCommand.Create(() => { })
        });
        _actionSource.AddOrUpdate(new MapAnchorActionViewModel
        {
            Order = 2,
            Icon = MaterialIconKind.Cab,
            Title = "Cab",
            Command = ReactiveCommand.Create(() => { })
        });*/
        Observable.Timer(TimeSpan.FromSeconds(10),TimeSpan.FromSeconds(3))
            .Take(10).ObserveOn(RxApp.MainThreadScheduler).Subscribe(x =>
            {
                _actionSource.AddOrUpdate(new MapAnchorActionViewModel
                {
                    Order = (int)x,
                    Icon = MaterialIconKind.Cab,
                    Title = $"Action {x +2}",
                    Command = ReactiveCommand.Create(() => { })
                });
            });
    }

    public override ReadOnlyObservableCollection<MapAnchorActionViewModel> Actions => _actions;
}