using Asv.Common;
using Avalonia.Controls;

namespace Asv.Avalonia.Map.Demo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        GoogleMapProvider.Instance.ApiKey = "AIzaSyAmO6pIPTz0Lt8lmYZEIAaixitKjq-4WlB";
        var mainMap = this.Get<MapView>("GMap");
        // MainMap.MapProvider = GMapProviders.BingHybridMap;
        mainMap.Position = new GeoPoint(55.1644, 61.4368, 190);
        GMaps.Instance.BoostCacheEngine = false;
        GMaps.Instance.CacheOnIdleRead = true;
        GMaps.Instance.UseMemoryCache = true;
    }
}
