
using Avalonia.Controls;

namespace Asv.Avalonia.Map.Demo.Menu.Actions.AnchorMover;

// [Export(FlightPageViewModel.UriString,typeof(IMapAction))]
// [Export(PlaningPageViewModel.UriString,typeof(IMapAction))]
// [PartCreationPolicy(CreationPolicy.NonShared)]
public class AnchorMoverActionViewModel : ViewModelBase
{
    private IMap _map;
    
    public AnchorMoverActionViewModel() : base("asv:shell.page.map.action.move-anchors")
    {
    }
    
    // public IMapAction Init(IMap context)
    // {
    //     _map = context;
    //     return this;
    // }
    bool IsInAnchorEditMode { get; set; }
    public IMap Map => _map;
    public Dock Dock { get; }
    public int Order => 0;
}