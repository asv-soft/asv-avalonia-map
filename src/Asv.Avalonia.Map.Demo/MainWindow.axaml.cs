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
            

            // this.WhenActivated(disp =>
            // {
            //     MainMap.Markers.AddMissionItem(new GMapMarker(MainMap.Position) { Shape = new Rectangle { Width = 10, Height = 10, Fill = Brushes.Red } });
            // });
            // this is only for MAP control disposing
            
        }

        public MapView MainMap { get; set; }
    }
}
