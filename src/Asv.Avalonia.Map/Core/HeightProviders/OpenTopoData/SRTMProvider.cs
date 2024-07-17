using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Asv.Common;
using Newtonsoft.Json;

namespace Asv.Avalonia.Map.HeightProviders;

public class SRTMProvider
{
    public async Task<GeoPoint> GetPointAltitude(GeoPoint point, DataSet dataSet = DataSet.Srtm30M, Interpolation interpolation = Interpolation.Bilinear)
    {
        var locationsString = $"{point.Latitude.ToString("0.000000", CultureInfo.InvariantCulture)},{point.Longitude.ToString("0.000000", CultureInfo.InvariantCulture)}";
        _request = new HttpRequestMessage(HttpMethod.Get,  $"http://api.opentopodata.org/v1/{dataSet.ToString().ToLower()}?locations={locationsString}&interpolation={interpolation.ToString().ToLower()}");
        var response = _client.Send(_request);
        var responseModel = JsonConvert.DeserializeObject<GetOpenTopoDataResponseModel>(await response.Content.ReadAsStringAsync());
        var elevation = responseModel?.Results[0].Elevation ?? 0.0;
        return new GeoPoint(point.Latitude, point.Longitude, elevation);
    }
    
    public async Task<ObservableCollection<GeoPoint>> GetPointAltitudeCollection(ObservableCollection<GeoPoint> listGeoPoints, DataSet dataSet = DataSet.Srtm30M,
        Interpolation interpolation = Interpolation.Bilinear)
    {
        var newListGeoPoint = new ObservableCollection<GeoPoint>();
        var locationsString = listGeoPoints.Aggregate(string.Empty,
            (current, item) => current + $"{item.Latitude.ToString("0.000000", CultureInfo.InvariantCulture)},{item.Longitude.ToString("0.000000", CultureInfo.InvariantCulture)}|");
        _request = new HttpRequestMessage( HttpMethod.Get, 
            $"http://api.opentopodata.org/v1/{dataSet.ToString().ToLower()}?locations={locationsString}&interpolation={interpolation.ToString().ToLower()}");
        var response = _client.Send(_request);
        var responseModel = JsonConvert.DeserializeObject<GetOpenTopoDataResponseModel>(await response.Content.ReadAsStringAsync());
        if (responseModel is null) return listGeoPoints;
        for (var i = 0; i < responseModel.Results.Count - 1; i++)
        {
            var oldLocation = listGeoPoints[i];
            var elevation = responseModel.Results[0].Elevation ?? 0.0;
           newListGeoPoint.Add(new GeoPoint(oldLocation.Latitude, oldLocation.Longitude, elevation));
        }
        return newListGeoPoint;
    }

    public enum Interpolation
    {
        Cubic,
        Nearest,
        Bilinear
    }

    public enum DataSet
    {
        Srtm30M,
        Srtm90M,
    }

    private readonly HttpClient _client = new();
    private HttpRequestMessage _request = new();

}