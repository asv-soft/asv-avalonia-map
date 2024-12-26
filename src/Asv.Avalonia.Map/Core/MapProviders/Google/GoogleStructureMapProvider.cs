using System.Collections.Generic;

namespace Asv.Avalonia.Map
{
    #region Geocode

    public class StrucGeocode
    {
        public List<Result>? Results { get; set; }
        public GeoCoderStatusCode Status { get; set; }
    }

    public class AddressComponent
    {
        public string? LongName { get; set; }
        public string? ShortName { get; set; }
        public List<string>? Types { get; set; }
    }

    public class Northeast
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Southwest
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Bounds
    {
        public Northeast? Northeast { get; set; }
        public Southwest? Southwest { get; set; }
    }

    public class Location
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Northeast2
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Southwest2
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Viewport
    {
        public Northeast2? Northeast { get; set; }
        public Southwest2? Southwest { get; set; }
    }

    public class Geometry
    {
        public Bounds? Bounds { get; set; }
        public Location? Location { get; set; }
        public string? LocationType { get; set; }
        public Viewport? Viewport { get; set; }
    }

    public class Result
    {
        public List<AddressComponent>? AddressComponents { get; set; }
        public string? FormattedAddress { get; set; }
        public Geometry? Geometry { get; set; }
        public string? PlaceId { get; set; }
        public List<string>? Types { get; set; }
    }

    #endregion

    #region Direction

    public class StrucDirection
    {
        public List<GeocodedWaypoint>? GeocodedWaypoints { get; set; }
        public List<Route>? Routes { get; set; }
        public DirectionsStatusCode Status { get; set; }
    }

    public class GeocodedWaypoint
    {
        public string? GeocoderStatus { get; set; }
        public string? PlaceId { get; set; }
        public List<string>? Types { get; set; }
    }

    public class Distance
    {
        public string? Text { get; set; }
        public int Value { get; set; }
    }

    public class Duration
    {
        public string? Text { get; set; }
        public int Value { get; set; }
    }

    public class EndLocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class StartLocation
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Distance2
    {
        public string? Text { get; set; }
        public int Value { get; set; }
    }

    public class Duration2
    {
        public string? Text { get; set; }
        public int Value { get; set; }
    }

    public class EndLocation2
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Polyline
    {
        public string? Points { get; set; }
    }

    public class StartLocation2
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Step
    {
        public Distance2? Distance { get; set; }
        public Duration2? Duration { get; set; }
        public EndLocation2? EndLocation { get; set; }
        public string? HtmlInstructions { get; set; }
        public Polyline? Polyline { get; set; }
        public StartLocation2? StartLocation { get; set; }
        public string? TravelMode { get; set; }
        public string? Maneuver { get; set; }
    }

    public class Leg
    {
        public Distance? Distance { get; set; }
        public Duration? Duration { get; set; }
        public string? EndAddress { get; set; }
        public EndLocation? EndLocation { get; set; }
        public string? StartAddress { get; set; }
        public StartLocation? StartLocation { get; set; }
        public List<Step>? Steps { get; set; }
        public List<object>? TrafficSpeedEntry { get; set; }
        public List<object>? ViaWaypoint { get; set; }
    }

    public class OverviewPolyline
    {
        public string? Points { get; set; }
    }

    public class Route
    {
        public Bounds? Bounds { get; set; }
        public string? Copyrights { get; set; }
        public List<Leg>? Legs { get; set; }
        public OverviewPolyline? OverviewPolyline { get; set; }
        public string? Summary { get; set; }
        public List<object>? Warnings { get; set; }
        public List<object>? WaypointOrder { get; set; }
    }

    #endregion

    #region Rute

    public class StrucRute
    {
        public List<GeocodedWaypoint>? GeocodedWaypoints { get; set; }
        public List<Route>? Routes { get; set; }
        public RouteStatusCode Status { get; set; }
        public Error? Error { get; set; }
    }

    #endregion

    #region Roads

    public class StrucRoads
    {
        public Error? Error { get; set; }

        public string? WarningMessage { get; set; }

        public List<SnappedPoint>? SnappedPoints { get; set; }

        public class SnappedPoint
        {
            public Location? Location1 { get; set; }
            public int OriginalIndex { get; set; }
            public string? PlaceId { get; set; }

            public class Location
            {
                public double Latitude { get; set; }
                public double Longitude { get; set; }
            }
        }
    }

    #endregion

    #region Error

    public class Error
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public string? Status { get; set; }
        public List<Detail>? Details { get; set; }
    }

    public class Detail
    {
        public string? Type { get; set; }
        public List<Link>? Links { get; set; }
    }

    public class Link
    {
        public string? Description { get; set; }
        public string? Url { get; set; }
    }

    #endregion
}
