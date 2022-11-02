using System.Collections.Generic;
using Asv.Gnss;

namespace Asv.Avalonia.Map
{
    /// <summary>
    ///     roads interface
    /// </summary>
    public interface RoadsProvider
    {
        MapRoute GetRoadsRoute(List<GeoPoint> points, bool interpolate);

        MapRoute GetRoadsRoute(string points, bool interpolate);
    }
}
