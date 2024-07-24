using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Avalonia.Map;

public abstract class HeightProviderBase
{
    protected HttpRequestMessage RequestMessage = new();
    protected readonly HttpClient Client = new();

    protected static string ToWebString(double latitude, double longitude)
    {
      return 
          $"{latitude.ToString("0.000000", CultureInfo.InvariantCulture)},{longitude.ToString("0.000000", CultureInfo.InvariantCulture)}";
    }

    /// <summary>
    /// Asynchronously retrieves the altitude information for a specific geographical point.
    /// This information is encapsulated in a GeoPoint object, which is wrapped in a Task for async processing.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.
    /// The Task.Result property returns a GeoPoint object that holds the altitude information.</returns>
    public abstract Task<GeoPoint> GetPointAltitude(GeoPoint point);

    /// <summary>
    /// Asynchronously retrieves a collection of geographical points with altitude information.
    /// The collection is observable, allowing automatic UI updates in response to collection changes.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.
    /// The Task.Result property returns an ObservableCollection of GeoPoint objects.</returns>
    public abstract Task<ObservableCollection<GeoPoint>> GetPointAltitudeCollection(
        ObservableCollection<GeoPoint> pointsCollection);
}