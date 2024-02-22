using Asv.Common;
using Avalonia.Controls;

namespace Asv.Avalonia.Map.Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GoogleMapProvider.Instance.ApiKey = "AIzaSyAmO6pIPTz0Lt8lmYZEIAaixitKjq-4WlB";
            MainMap = this.Get<MapView>("GMap");
            // MainMap.MapProvider = GMapProviders.BingHybridMap;
            MainMap.Position = new GeoPoint(55.1644, 61.4368, 190);
            GMaps.Instance.BoostCacheEngine = false;
            GMaps.Instance.CacheOnIdleRead = true;
            GMaps.Instance.UseMemoryCache = true;
            
            
        }
        
        
        public MapView MainMap { get; set; }
    }
}
