using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Asv.Common;
using Newtonsoft.Json;

namespace Asv.Avalonia.Map.HeightProviders;

public class AsterProvider: HeightProviderBase
{
    public override async Task<GeoPoint> GetPointAltitude(GeoPoint point)
    {
        var locationsString = ToWebString(point.Latitude, point.Longitude);
        var request = new HttpRequestMessage(HttpMethod.Get,  $"http://api.opentopodata.org/v1/aster30m?locations={locationsString}");
        var response = Client.Send(request);
        var responseModel = JsonConvert.DeserializeObject<GetOpenTopoDataResponseModel>(await response.Content.ReadAsStringAsync());
        var elevation = responseModel?.Results[0].Elevation ?? 0.0;
        return new GeoPoint(point.Latitude, point.Longitude, elevation);
    }

    public override async Task<ObservableCollection<GeoPoint>> GetPointAltitudeCollection(ObservableCollection<GeoPoint> pointsCollection)
    {
        var newListGeoPoint = new ObservableCollection<GeoPoint>();
        var locationsString = pointsCollection.Aggregate(string.Empty,
            (current, item) => current + ToWebString(item.Latitude, item.Longitude))+"|";
        var request = new HttpRequestMessage( HttpMethod.Get, 
            $"http://api.opentopodata.org/v1/aster30m?locations={locationsString}");
        var response = Client.Send(request);
        var responseModel = JsonConvert.DeserializeObject<GetOpenTopoDataResponseModel>(await response.Content.ReadAsStringAsync());
        if (responseModel is null) return pointsCollection;
        for (var i = 0; i < responseModel.Results.Count - 1; i++)
        {
            var oldLocation = pointsCollection[i];
            var elevation = responseModel.Results[0].Elevation ?? 0.0;
            newListGeoPoint.Add(new GeoPoint(oldLocation.Latitude, oldLocation.Longitude, elevation));
        }
        return newListGeoPoint;
    }
}