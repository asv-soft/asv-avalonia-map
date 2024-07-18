using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Avalonia.GMap.Demo;
using Asv.Avalonia.Map.HeightProviders;
using Asv.Common;
using Avalonia.Media;
using DynamicData.Binding;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.Map.Demo;

public class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        CurrentMapProvider = GMapProviders.GoogleMap;
        SelectedAnchorVariant = AnchorViewModels[0];
        _markers = new ObservableCollection<MapAnchorViewModel>
        {
            new()
            {
                IsEditable = true,
                ZOrder = 0,
                OffsetX = OffsetXEnum.Left,
                OffsetY = OffsetYEnum.Top,
                IsSelected = false,
                IsVisible = true,
                Icon = MaterialIconKind.Navigation,
                Size = 32,
                BaseSize = 32,
                IconBrush = Brushes.LightSeaGreen,
                Title = "Hello!!!"
            }
        };
        _markers.Add(new RulerAnchor("1", Ruler, RulerPosition.Start));
        _markers.Add(new RulerAnchor("2", Ruler, RulerPosition.Stop));
        _markers.Add(new RulerPolygon(Ruler));
        this.WhenValueChanged(vm => vm.IsInAnchorEditMode).Subscribe(v =>
        {
            foreach (var marker in _markers)
                if (marker.IsEditable)
                    marker.IsInEditMode = v;
        });
        this.WhenValueChanged(vm => vm.IsRulerEnabled).Subscribe(v => SetUpRuler(v));
        AddAnchorCommand = ReactiveCommand.CreateFromTask(AddNewAnchor);
        AddAnchorsAreaCommand = ReactiveCommand.Create(AddAnchorsArea);
        RemoveAllAnchorsCommand = ReactiveCommand.Create(RemoveAllAnchors);
        SelectedItem = Markers[0];
        var provider = new AsterHeightProvider();
        this.WhenValueChanged(vm => vm.SelectedItem.Location).Subscribe(x =>
        {
            SelectedItem.Description =
                $@"Lat:{x.Latitude:0.000000},Lon: {x.Longitude:0.000000},Alt: {x.Altitude}m";
        });
        this.WhenValueChanged(vm => vm.SelectedItem.IsItemDragging).Subscribe(x =>
        {
            if (SelectedItem.IsItemDragging) return;
            var locationWithAlt = provider.GetPointAltitude(SelectedItem.Location).Result;
            SelectedItem.Location = locationWithAlt;
            SelectedItem.Description =
                $@"Lat:{locationWithAlt.Latitude:0.000000},Lon: {locationWithAlt.Longitude:0.000000},Alt: {locationWithAlt.Altitude}m";
        });
    }

    #region Anchors Actions

    public Ruler Ruler = new();
    [Reactive] public bool IsInAnchorEditMode { get; set; }
    [Reactive] public bool IsRulerEnabled { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> AddAnchorCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> AddAnchorsAreaCommand { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> RemoveAllAnchorsCommand { get; set; }
    [Reactive] public MapAnchorViewModel SelectedAnchorVariant { get; set; }
    public IEnumerable<GMapProvider> AvailableProviders => GMapProviders.List;
    [Reactive] public GMapProvider CurrentMapProvider { get; set; }

    private void RemoveAllAnchors()
    {
        _markers.Clear();
    }

    private CancellationTokenSource _tokenSource = new();

    private async void SetUpRuler(bool isEnabled)
    {
        var polygon = _markers.FirstOrDefault(x => x is RulerPolygon) as RulerPolygon;
        if (polygon is null)
        {
            polygon = new RulerPolygon(Ruler);
            _markers.Add(new RulerAnchor("1", Ruler, RulerPosition.Start));
            _markers.Add(new RulerAnchor("2", Ruler, RulerPosition.Stop));
            _markers.Add(polygon);
        }

        _tokenSource.Cancel();
        _tokenSource = new CancellationTokenSource();
        if (isEnabled)
            try
            {
                var start = await ShowTargetDialog("Set a start point",
                    _tokenSource.Token);
                if (start.Equals(GeoPoint.NaN))
                {
                    IsRulerEnabled = false;
                    return;
                }

                var stop = await ShowTargetDialog("Set a end point",
                    _tokenSource.Token);
                if (stop.Equals(GeoPoint.NaN))
                {
                    IsRulerEnabled = false;
                    return;
                }

                polygon.Ruler.Value.Start.OnNext(start);
                polygon.Ruler.Value.Stop.OnNext(stop);
            }
            catch (TaskCanceledException)
            {
                return;
            }

        polygon.Ruler.Value.IsVisible.OnNext(isEnabled);
    }

    private async Task AddNewAnchor()
    {
        await _tokenSource.CancelAsync();
        _tokenSource = new CancellationTokenSource();
        try
        {
            var userPoint = await ShowTargetDialog("Set a point",
                _tokenSource.Token);
            var heightProvider = new SRTMHeightProvider();
            var pointWithAltitude = heightProvider.GetPointAltitude(userPoint).Result;
            if (SelectedAnchorVariant is VehicleAnchorViewModel)
            {
                _markers.Add(new VehicleAnchorViewModel
                {
                    Location = pointWithAltitude,
                    Description =
                        $@"{pointWithAltitude.Latitude}, {pointWithAltitude.Longitude}, {pointWithAltitude.Altitude}"
                });
            }
            else
            {
                var newAnchor = new MapAnchorViewModel
                {
                    IsEditable = SelectedAnchorVariant.IsEditable,
                    ZOrder = SelectedAnchorVariant.ZOrder,
                    OffsetX = SelectedAnchorVariant.OffsetX,
                    OffsetY = SelectedAnchorVariant.OffsetY,
                    IsSelected = SelectedAnchorVariant.IsSelected,
                    IsVisible = SelectedAnchorVariant.IsVisible,
                    Icon = SelectedAnchorVariant.Icon,
                    Size = SelectedAnchorVariant.Size,
                    IconBrush = SelectedAnchorVariant.IconBrush,
                    Title = SelectedAnchorVariant.Title,
                    Location = pointWithAltitude,
                    Description =
                        $@"{pointWithAltitude.Latitude:0.000000}, {pointWithAltitude.Longitude:0.000000}, {pointWithAltitude.Altitude}"
                };
                _markers.Add(newAnchor);
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    private async void AddAnchorsArea()
    {
        await _tokenSource.CancelAsync();
        _tokenSource = new CancellationTokenSource();
        try
        {
            var userPoint = await ShowTargetDialog("Set a point",
                _tokenSource.Token);
            ObservableCollection<GeoPoint> pointsCollection = new ObservableCollection<GeoPoint>()
            {
                new(userPoint.Latitude + 0.0001, userPoint.Longitude + 0.0001, userPoint.Altitude),
                new(userPoint.Latitude + 0.0001, userPoint.Longitude - 0.0001, userPoint.Altitude),
                new(userPoint.Latitude + 0.0002, userPoint.Longitude + 0.0002, userPoint.Altitude),
                new(userPoint.Latitude + 0.0002, userPoint.Longitude - 0.0002, userPoint.Altitude),
                new(userPoint.Latitude + 0.0003, userPoint.Longitude + 0.0003, userPoint.Altitude),
                new(userPoint.Latitude + 0.0003, userPoint.Longitude - 0.0003, userPoint.Altitude),
                new(userPoint.Latitude + 0.0004, userPoint.Longitude + 0.0004, userPoint.Altitude),
                new(userPoint.Latitude + 0.0004, userPoint.Longitude - 0.0004, userPoint.Altitude),
                new(userPoint.Latitude + 0.0004, userPoint.Longitude - 0.0004, userPoint.Altitude),
                new(userPoint.Latitude + 0.0004, userPoint.Longitude + 0.0004, userPoint.Altitude),
                new(userPoint.Latitude + 0.0004, userPoint.Longitude + 0.0001, userPoint.Altitude),
                new(userPoint.Latitude + 0.0004, userPoint.Longitude - 0.0001, userPoint.Altitude),
                new(userPoint.Latitude + 0.0004, userPoint.Longitude, userPoint.Altitude),
                new(userPoint.Latitude + 0.0005, userPoint.Longitude, userPoint.Altitude),
                new(userPoint.Latitude + 0.0005, userPoint.Longitude + 0.0005, userPoint.Altitude),
                new(userPoint.Latitude + 0.0005, userPoint.Longitude - 0.0005, userPoint.Altitude),
                new(userPoint.Latitude + 0.0006, userPoint.Longitude - 0.0004, userPoint.Altitude),
                new(userPoint.Latitude + 0.0006, userPoint.Longitude + 0.0004, userPoint.Altitude),
                new(userPoint.Latitude + 0.0007, userPoint.Longitude - 0.0003, userPoint.Altitude),
                new(userPoint.Latitude + 0.0007, userPoint.Longitude + 0.0003, userPoint.Altitude),
                new(userPoint.Latitude + 0.0008, userPoint.Longitude + 0.0002, userPoint.Altitude),
                new(userPoint.Latitude + 0.0008, userPoint.Longitude - 0.0002, userPoint.Altitude),
                new(userPoint.Latitude + 0.0008, userPoint.Longitude + 0.0001, userPoint.Altitude),
                new(userPoint.Latitude + 0.0008, userPoint.Longitude - 0.0001, userPoint.Altitude),
                new(userPoint.Latitude + 0.0008, userPoint.Longitude, userPoint.Altitude),
                new(userPoint.Latitude, userPoint.Longitude, userPoint.Altitude),
                new(userPoint.Latitude + 0.0015, userPoint.Longitude - 0.0015, userPoint.Altitude),
                new(userPoint.Latitude + 0.0015, userPoint.Longitude, userPoint.Altitude),
                new(userPoint.Latitude + 0.0015, userPoint.Longitude + 0.0015, userPoint.Altitude),
                new(userPoint.Latitude + 0.0015, userPoint.Longitude - 0.001, userPoint.Altitude),
                new(userPoint.Latitude + 0.0015, userPoint.Longitude + 0.001, userPoint.Altitude),
                new(userPoint.Latitude, userPoint.Longitude + 0.0015, userPoint.Altitude),
                new(userPoint.Latitude, userPoint.Longitude - 0.0015, userPoint.Altitude),
                new(userPoint.Latitude + 0.001, userPoint.Longitude + 0.0015, userPoint.Altitude),
                new(userPoint.Latitude - 0.001, userPoint.Longitude - 0.0015, userPoint.Altitude),
                new(userPoint.Latitude - 0.0015, userPoint.Longitude - 0.0015, userPoint.Altitude),
                new(userPoint.Latitude - 0.0015, userPoint.Longitude - 0.001, userPoint.Altitude),
                new(userPoint.Latitude - 0.0015, userPoint.Longitude + 0.001, userPoint.Altitude),
                new(userPoint.Latitude - 0.0015, userPoint.Longitude, userPoint.Altitude),
                new(userPoint.Latitude - 0.0015, userPoint.Longitude + 0.0015, userPoint.Altitude),
            };
            var provider = new SRTMHeightProvider();
            pointsCollection = await provider.GetPointAltitudeCollection(pointsCollection);
            foreach (var item in pointsCollection)
            {
                _markers.Add(new MapAnchorViewModel()
                {
                    Stroke = Brushes.Aqua,
                    StrokeThickness = 2,
                    IsEditable = true,
                    ZOrder = 0,
                    OffsetX = OffsetXEnum.Center,
                    OffsetY = OffsetYEnum.Bottom,
                    IsSelected = false,
                    IsVisible = true,
                    Icon = MaterialIconKind.MapMarker,
                    Size = 34,
                    BaseSize = 34,
                    IconBrush = Brushes.Crimson,
                    Location = item,
                    Description = $@"Lat:{item.Latitude:0.000000},Lon: {item.Longitude:0.000000},Alt: {item.Altitude}m"
                });
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    public List<MapAnchorViewModel> AnchorViewModels =>
    [
        new MapAnchorViewModel
        {
            Stroke = Brushes.Aqua,
            StrokeThickness = 2,
            IsEditable = true,
            ZOrder = 0,
            OffsetX = OffsetXEnum.Center,
            OffsetY = OffsetYEnum.Bottom,
            IsSelected = false,
            IsVisible = true,
            Icon = MaterialIconKind.MapMarker,
            Size = 34,
            BaseSize = 34,
            IconBrush = Brushes.Crimson,
            Title = "Map Marker"
        },
        new VehicleAnchorViewModel()
    ];

    #endregion

    #region Map Properties

    public ObservableCollection<MapAnchorViewModel> Markers => _markers;
    [Reactive] public MapAnchorViewModel SelectedItem { get; set; }
    [Reactive] public GeoPoint Center { get; set; }
    [Reactive] public GeoPoint DialogTarget { get; set; }
    [Reactive] public bool IsInDialogMode { get; set; }
    [Reactive] public string DialogText { get; set; }
    private readonly ObservableCollection<MapAnchorViewModel> _markers;

    private async Task<GeoPoint> ShowTargetDialog(string text, CancellationToken cancel)
    {
        DialogText = text;
        IsInDialogMode = true;
        var tcs = new TaskCompletionSource();
        await using var c1 = cancel.Register(() => tcs.TrySetCanceled());
        this.WhenAnyValue(_ => _.IsInDialogMode).Where(_ => IsInDialogMode == false)
            .Subscribe(_ => tcs.TrySetResult(), cancel);
        await tcs.Task;
        return DialogTarget;
    }

    #endregion
}