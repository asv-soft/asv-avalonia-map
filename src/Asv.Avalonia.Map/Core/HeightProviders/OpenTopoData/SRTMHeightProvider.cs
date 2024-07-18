﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Asv.Common;
using Newtonsoft.Json.Linq;

namespace Asv.Avalonia.Map.HeightProviders;

// ReSharper disable once InconsistentNaming
public class SRTMHeightProvider : HeightProviderBase
{
    public override async Task<GeoPoint> GetPointAltitude(GeoPoint point)
    {
        var locationsString = ToWebString(point.Latitude,point.Longitude);
        RequestMessage = new HttpRequestMessage(HttpMethod.Get,  $"http://api.opentopodata.org/v1/{DataSet.ToString().ToLower()}?locations={locationsString}&interpolation={Interpolation.ToString().ToLower()}");
        var response =  Client.Send(RequestMessage);
        var content = await response.Content.ReadAsStringAsync();
        var elevationString = JObject.Parse(content)["results"]?[0]?["elevation"]?.ToString();
        double.TryParse(elevationString, out var elevation);
        return new GeoPoint(point.Latitude, point.Longitude, elevation);
    }
    public override async Task<ObservableCollection<GeoPoint>> GetPointAltitudeCollection(ObservableCollection<GeoPoint> pointsCollection)
    {
        var newListGeoPoint = new ObservableCollection<GeoPoint>();
        var locationsString = pointsCollection.Aggregate(string.Empty,
            (current, item) => current + $"{ToWebString(item.Latitude,item.Longitude)}|");
        RequestMessage = new HttpRequestMessage( HttpMethod.Get, 
            $"http://api.opentopodata.org/v1/{DataSet.ToString().ToLower()}?locations={locationsString}&interpolation={Interpolation.ToString().ToLower()}");
        var response = Client.Send(RequestMessage);
        var content = await response.Content.ReadAsStringAsync();
        var jObject = JObject.Parse(content)["results"];
        if (jObject is null) return pointsCollection;
        for (var i = 0; i < jObject.Count(); i++)
        {
            var elevationString = jObject[i]?["elevation"]?.ToString();
            double.TryParse(elevationString, out var elevation);
            newListGeoPoint.Add(new GeoPoint(pointsCollection[i].Latitude, pointsCollection[i].Longitude, elevation));
        }
        return newListGeoPoint;
    }
    
    public DataSet DataSet { get; set; } = DataSet.Srtm30M;
    public Interpolation Interpolation { get; set; } = Interpolation.Cubic;

}