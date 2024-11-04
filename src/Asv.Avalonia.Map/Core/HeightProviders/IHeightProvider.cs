using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Asv.Common;

namespace Asv.Avalonia.Map;

public enum Interpolation
{
    Cubic,
    Nearest,
    Bilinear,
}

public enum DataSet
{
    Srtm30M,
    Srtm90M,
}

public interface IHeightProvider
{
    /// <summary>
    /// Asynchronously retrieves the altitude information for a specific geographical point.
    /// This information is encapsulated in a GeoPoint object, which is wrapped in a Task for async processing.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.
    /// The Task.Result property returns a GeoPoint object that holds the altitude information.</returns>
    public Task<GeoPoint> GetPointAltitude(GeoPoint point);

    /// <summary>
    /// Asynchronously retrieves a collection of geographical points with altitude information.
    /// The collection is observable, allowing automatic UI updates in response to collection changes.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.
    /// The Task.Result property returns an ObservableCollection of GeoPoint objects.</returns>
    public Task<ObservableCollection<GeoPoint>> GetPointAltitudeCollection(
        ObservableCollection<GeoPoint> pointsCollection
    );
}
