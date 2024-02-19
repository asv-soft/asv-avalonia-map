using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Asv.Drones.Gui.Core;
using DynamicData.Binding;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.Map.Demo.Menu.Actions.Ruler;


public class MapRulerActionViewModel:MapActionBase
{
    
    public MapRulerActionViewModel() : base("asv:shell.page.map.action.ruler")
    {
        this.WhenValueChanged(_ => _.IsRulerEnabled)
            .Subscribe(SetUpRuler)
            .DisposeItWith(Disposable);
    }
    
    private async void SetUpRuler(bool isEnabled)
    {
        if (Map == null) return;
        var polygon = Map.Markers.FirstOrDefault(x => x is RulerPolygon) as RulerPolygon;
        if (polygon == null) return;
        
        _tokenSource.Cancel();
        _tokenSource = new CancellationTokenSource();
        
        if(isEnabled)
        {
            try
            {
                var start = await Map.ShowTargetDialog( "start point",
                    _tokenSource.Token);
                if (start.Equals(GeoPoint.NaN))
                {
                    IsRulerEnabled = false;
                    return;
                }
                var stop = await Map.ShowTargetDialog("end point",
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
        }
        
        polygon.Ruler.Value.IsVisible.OnNext(isEnabled);
    }
    
    private static CancellationTokenSource _tokenSource = new ();
    
    [Reactive] 
    public bool IsRulerEnabled { get; set; }
}
