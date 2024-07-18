using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Avalonia.Map.HeightProviders;

public abstract class HeightProviderBase 
{
    protected HttpRequestMessage RequestMessage = new();
    protected readonly HttpClient Client = new ();
    protected static string ToWebString(double latitude, double longitude)
    {
      return $"{latitude.ToString("0.000000", CultureInfo.InvariantCulture)},{longitude.ToString("0.000000", CultureInfo.InvariantCulture)}";
    }
     public abstract Task<GeoPoint> GetPointAltitude(GeoPoint point);
     public abstract Task<ObservableCollection<GeoPoint>> GetPointAltitudeCollection(ObservableCollection<GeoPoint> pointsCollection);

}