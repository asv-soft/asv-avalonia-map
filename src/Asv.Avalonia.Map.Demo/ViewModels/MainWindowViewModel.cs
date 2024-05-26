using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Avalonia.GMap.Demo;
using Asv.Common;
using Avalonia.Media;
using DynamicData;
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
        AddAnchor = ReactiveCommand.Create(AddNewAnchor);
        RemoveAllAnchorsCommand = ReactiveCommand.Create(RemoveAllAnchors);
    }

    #region Anchors Actions

    public Ruler Ruler = new();
    [Reactive] public bool IsInAnchorEditMode { get; set; }
    [Reactive] public bool IsRulerEnabled { get; set; }
    [Reactive] public ReactiveCommand<Unit, Unit> AddAnchor { get; set; }
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

    private async void AddNewAnchor()
    {
        await _tokenSource.CancelAsync();
        _tokenSource = new CancellationTokenSource();

        try
        {
            var userPoint = await ShowTargetDialog("Set a point",
                _tokenSource.Token);

            if (SelectedAnchorVariant is VehicleAnchorViewModel)
            {
                _markers.Add(new VehicleAnchorViewModel
                {
                    Location = userPoint
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
                    Location = userPoint
                };
                _markers.Add(newAnchor);
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

    public MapAnchorViewModel SelectedItem { get; set; }

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