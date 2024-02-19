using System;
using System.Reactive.Linq;
using Asv.Common;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI;

namespace Asv.Avalonia.Map.Demo.Menu.Actions.Ruler;

public enum RulerPosition
{
    Start,
    Stop
}

public class RulerAnchor : MapAnchorBase
{
    public RulerAnchor(string id, Ruler ruler, RulerPosition rulerPosition) : base(new Uri($"{MapPageViewModel.Uri}layer/ruler/{id}"))
    {
        Size = 48;
        OffsetX = OffsetXEnum.Center;
        OffsetY = OffsetYEnum.Bottom;
        StrokeThickness = 1;
        IconBrush = Brushes.Indigo;
        Stroke = Brushes.White;
        IsVisible = false;
        Icon = MaterialIconKind.MapMarker;
        IsEditable = true;
        
        ruler.IsVisible.Where(_ => _.HasValue).Subscribe(_ => IsVisible = _.Value).DisposeItWith(Disposable);

        if (rulerPosition == RulerPosition.Stop)
        {
            ruler.Distance.Subscribe(_ => Title = _.ToString());
        }

        var isLocationInternalChanged = false;
        var point = rulerPosition == RulerPosition.Start ? ruler.Start : ruler.Stop;

        point.Where(_ => _.HasValue)
             .Subscribe(_ =>
             {
                 isLocationInternalChanged = true;
                 Location = _.Value;
                 isLocationInternalChanged = false;
             })
             .DisposeItWith(Disposable);
        
        this.WhenAnyValue(_ => _.Location)
            .Where(_ => !isLocationInternalChanged)
            .Subscribe(_ => point.OnNext(_))
            .DisposeItWith(Disposable);
    }
}