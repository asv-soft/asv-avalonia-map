using Asv.Avalonia.Map.Demo.Services;
using Asv.Drones.Gui.Core;
using DynamicData;

namespace Asv.Avalonia.Map.Demo.Menu.Actions.Ruler;


public class RulerMapLayerProvider : ViewModelProviderBase<IMapAnchor>
{
    
    public RulerMapLayerProvider()
    {
        var ruler = new Ruler();
        
        Source.AddOrUpdate(new RulerAnchor("1", ruler, RulerPosition.Start ));
        Source.AddOrUpdate(new RulerAnchor("2", ruler, RulerPosition.Stop));
        Source.AddOrUpdate(new RulerPolygon(ruler));
    }
}