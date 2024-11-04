﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Asv.Common;
using static System.Math;

namespace Asv.Avalonia.Map
{
    /// <summary>
    ///     defines projection
    /// </summary>
    public abstract class PureProjection
    {
        private readonly List<Dictionary<GeoPoint, GPoint>> _fromLatLngToPixelCache = new List<
            Dictionary<GeoPoint, GPoint>
        >(33);

        private readonly List<Dictionary<GPoint, GeoPoint>> _fromPixelToLatLngCache = new List<
            Dictionary<GPoint, GeoPoint>
        >(33);

        public PureProjection()
        {
            for (int i = 0; i < _fromLatLngToPixelCache.Capacity; i++)
            {
                _fromLatLngToPixelCache.Add(new Dictionary<GeoPoint, GPoint>());
                _fromPixelToLatLngCache.Add(new Dictionary<GPoint, GeoPoint>());
            }
        }

        /// <summary>
        ///     size of tile
        /// </summary>
        public abstract GSize TileSize { get; }

        /// <summary>
        ///     Semi-major axis of ellipsoid, in meters
        /// </summary>
        public abstract double Axis { get; }

        /// <summary>
        ///     Flattening of ellipsoid
        /// </summary>
        public abstract double Flattening { get; }

        /// <summary>
        ///     get pixel coordinates from lat/lng
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public abstract GPoint FromLatLngToPixel(double lat, double lng, int zoom);

        /// <summary>
        ///     gets lat/lng coordinates from pixel coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public abstract GeoPoint FromPixelToLatLng(long x, long y, int zoom);

        public GPoint FromLatLngToPixel(GeoPoint p, int zoom)
        {
            return FromLatLngToPixel(p, zoom, false);
        }

        /// <summary>
        ///     get pixel coordinates from lat/lng
        /// </summary>
        /// <param name="p"></param>
        /// <param name="zoom"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public GPoint FromLatLngToPixel(GeoPoint p, int zoom, bool useCache)
        {
            if (useCache)
            {
                var ret = GPoint.Empty;
                if (!_fromLatLngToPixelCache[zoom].TryGetValue(p, out ret))
                {
                    ret = FromLatLngToPixel(p.Latitude, p.Longitude, zoom);
                    _fromLatLngToPixelCache[zoom].Add(p, ret);

                    // for reverse cache
                    if (!_fromPixelToLatLngCache[zoom].ContainsKey(ret))
                    {
                        _fromPixelToLatLngCache[zoom].Add(ret, p);
                    }

                    Debug.WriteLine(
                        "FromLatLngToPixelCache[" + zoom + "] added " + p + " with " + ret
                    );
                }

                return ret;
            }
            else
            {
                return FromLatLngToPixel(p.Latitude, p.Longitude, zoom);
            }
        }

        public GeoPoint FromPixelToLatLng(GPoint p, int zoom)
        {
            return FromPixelToLatLng(p, zoom, false);
        }

        /// <summary>
        ///     gets lat/lng coordinates from pixel coordinates
        /// </summary>
        /// <param name="p"></param>
        /// <param name="zoom"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public GeoPoint FromPixelToLatLng(GPoint p, int zoom, bool useCache)
        {
            if (useCache)
            {
                var ret = GeoPoint.ZeroWithAlt;
                if (!_fromPixelToLatLngCache[zoom].TryGetValue(p, out ret))
                {
                    ret = FromPixelToLatLng(p.X, p.Y, zoom);
                    _fromPixelToLatLngCache[zoom].Add(p, ret);

                    // for reverse cache
                    if (!_fromLatLngToPixelCache[zoom].ContainsKey(ret))
                    {
                        _fromLatLngToPixelCache[zoom].Add(ret, p);
                    }

                    Debug.WriteLine(
                        "FromPixelToLatLngCache[" + zoom + "] added " + p + " with " + ret
                    );
                }

                return ret;
            }
            else
            {
                return FromPixelToLatLng(p.X, p.Y, zoom);
            }
        }

        /// <summary>
        ///     gets tile coorddinate from pixel coordinates
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual GPoint FromPixelToTileXY(GPoint p)
        {
            return new GPoint(p.X / TileSize.Width, p.Y / TileSize.Height);
        }

        /// <summary>
        ///     gets pixel coordinate from tile coordinate
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual GPoint FromTileXYToPixel(GPoint p)
        {
            return new GPoint(p.X * TileSize.Width, p.Y * TileSize.Height);
        }

        /// <summary>
        ///     min. tile in tiles at custom zoom level
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public abstract GSize GetTileMatrixMinXY(int zoom);

        /// <summary>
        ///     max. tile in tiles at custom zoom level
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public abstract GSize GetTileMatrixMaxXY(int zoom);

        /// <summary>
        ///     gets matrix size in tiles
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public virtual GSize GetTileMatrixSizeXY(int zoom)
        {
            var sMin = GetTileMatrixMinXY(zoom);
            var sMax = GetTileMatrixMaxXY(zoom);

            return new GSize(sMax.Width - sMin.Width + 1, sMax.Height - sMin.Height + 1);
        }

        /// <summary>
        ///     tile matrix size in pixels at custom zoom level
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public long GetTileMatrixItemCount(int zoom)
        {
            var s = GetTileMatrixSizeXY(zoom);
            return s.Width * s.Height;
        }

        /// <summary>
        ///     gets matrix size in pixels
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public virtual GSize GetTileMatrixSizePixel(int zoom)
        {
            var s = GetTileMatrixSizeXY(zoom);
            return new GSize(s.Width * TileSize.Width, s.Height * TileSize.Height);
        }

        /// <summary>
        ///     gets all tiles in rect at specific zoom
        /// </summary>
        public List<GPoint> GetAreaTileList(RectLatLng rect, int zoom, int padding)
        {
            var topLeft = FromPixelToTileXY(FromLatLngToPixel(rect.LocationTopLeft, zoom));
            var rightBottom = FromPixelToTileXY(FromLatLngToPixel(rect.LocationRightBottom, zoom));

            long x = Max(0, topLeft.X - padding);
            long toX = rightBottom.X + padding;
            long y0 = Max(0, topLeft.Y - padding);
            long toY = rightBottom.Y + padding;

            var list = new List<GPoint>((int)((toX - x + 1) * (toY - y0 + 1)));

            for (; x <= toX; x++)
            {
                for (long y = y0; y <= toY; y++)
                {
                    list.Add(new GPoint(x, y));
                }
            }

            return list;
        }

        /// <summary>
        ///     The ground resolution indicates the distance (in meters) on the ground that’s represented by a single pixel in the
        ///     map.
        ///     For example, at a ground resolution of 10 meters/pixel, each pixel represents a ground distance of 10 meters.
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public virtual double GetGroundResolution(int zoom, double latitude)
        {
            return Cos(latitude * (PI / 180)) * 2 * PI * Axis / GetTileMatrixSizePixel(zoom).Width;
        }

        /// <summary>
        ///     gets boundaries
        /// </summary>
        public virtual RectLatLng Bounds
        {
            get { return RectLatLng.FromLTRB(-180, 90, 180, -90); }
        }

        #region -- math functions --

        /// <summary>
        ///     Half of PI
        /// </summary>
        protected static readonly double HalfPi = PI * 0.5;

        /// <summary>
        ///     PI * 2
        /// </summary>
        protected static readonly double TwoPi = PI * 2.0;

        /// <summary>
        ///     EPSLoN
        /// </summary>
        protected static readonly double Epsilon = 1.0e-10;

        /// <summary>
        ///     MAX_VAL
        /// </summary>
        protected const double MaxVal = 4;

        /// <summary>
        ///     MAXLONG
        /// </summary>
        protected static readonly double MaxLong = 0x7FFFFFFF;

        /// <summary>
        ///     DBLLONG
        /// </summary>
        protected static readonly double DblLong = 4.61168601e18;

        static readonly double R2D = 180 / PI;
        static readonly double D2R = PI / 180;

        public static double DegreesToRadians(double deg)
        {
            return D2R * deg;
        }

        public static double RadiansToDegrees(double rad)
        {
            return R2D * rad;
        }

        /// <summary>
        ///     return the sign of an argument
        /// </summary>
        protected static double Sign(double x)
        {
            return x < 0.0 ? -1 : 1;
        }

        protected static double AdjustLongitude(double x)
        {
            long count = 0;
            while (true)
            {
                if (Abs(x) <= PI)
                    break;
                else if ((long)Abs(x / PI) < 2)
                    x = x - Sign(x) * TwoPi;
                else if ((long)Abs(x / TwoPi) < MaxLong)
                {
                    x = x - (long)(x / TwoPi) * TwoPi;
                }
                else if ((long)Abs(x / (MaxLong * TwoPi)) < MaxLong)
                {
                    x = x - (long)(x / (MaxLong * TwoPi)) * (TwoPi * MaxLong);
                }
                else if ((long)Abs(x / (DblLong * TwoPi)) < MaxLong)
                {
                    x = x - (long)(x / (DblLong * TwoPi)) * (TwoPi * DblLong);
                }
                else
                    x = x - Sign(x) * TwoPi;

                count++;
                if (count > MaxVal)
                    break;
            }

            return x;
        }

        /// <summary>
        ///     calculates the sine and cosine
        /// </summary>
        protected static void SinCos(double val, out double sin, out double cos)
        {
            sin = Sin(val);
            cos = Cos(val);
        }

        /// <summary>
        ///     computes the constants e0, e1, e2, and e3 which are used
        ///     in a series for calculating the distance along a meridian.
        /// </summary>
        /// <param name="x">represents the eccentricity squared</param>
        /// <returns></returns>
        protected static double E0Fn(double x)
        {
            return 1.0 - 0.25 * x * (1.0 + x / 16.0 * (3.0 + 1.25 * x));
        }

        protected static double E1Fn(double x)
        {
            return 0.375 * x * (1.0 + 0.25 * x * (1.0 + 0.46875 * x));
        }

        protected static double E2Fn(double x)
        {
            return 0.05859375 * x * x * (1.0 + 0.75 * x);
        }

        protected static double E3Fn(double x)
        {
            return x * x * x * (35.0 / 3072.0);
        }

        /// <summary>
        ///     computes the value of M which is the distance along a meridian
        ///     from the Equator to latitude phi.
        /// </summary>
        protected static double Mlfn(double e0, double e1, double e2, double e3, double phi)
        {
            return e0 * phi - e1 * Sin(2.0 * phi) + e2 * Sin(4.0 * phi) - e3 * Sin(6.0 * phi);
        }

        /// <summary>
        ///     calculates UTM zone number
        /// </summary>
        /// <param name="lon">Longitude in degrees</param>
        /// <returns></returns>
        protected static long GetUTMZone(double lon)
        {
            return (long)((lon + 180.0) / 6.0 + 1.0);
        }

        /// <summary>
        ///     Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        protected static double Clip(double n, double minValue, double maxValue)
        {
            return Min(Max(n, minValue), maxValue);
        }

        /// <summary>
        ///     distance (in km) between two points specified by latitude/longitude
        ///     The Haversine formula, http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double GetDistance(GeoPoint p1, GeoPoint p2)
        {
            double dLat1InRad = p1.Latitude * (PI / 180);
            double dLong1InRad = p1.Longitude * (PI / 180);
            double dLat2InRad = p2.Latitude * (PI / 180);
            double dLong2InRad = p2.Longitude * (PI / 180);
            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;
            double a =
                Pow(Sin(dLatitude / 2), 2)
                + Cos(dLat1InRad) * Cos(dLat2InRad) * Pow(Sin(dLongitude / 2), 2);
            double c = 2 * Atan2(Sqrt(a), Sqrt(1 - a));
            double dDistance = Axis / 1000.0 * c;
            return dDistance;
        }

        public double GetDistanceInPixels(GPoint point1, GPoint point2)
        {
            double a = point2.X - point1.X;
            double b = point2.Y - point1.Y;

            return Sqrt(a * a + b * b);
        }

        /// <summary>
        ///     Accepts two coordinates in degrees.
        /// </summary>
        /// <returns>A double value in degrees. From 0 to 360.</returns>
        public double GetBearing(GeoPoint p1, GeoPoint p2)
        {
            double latitude1 = DegreesToRadians(p1.Latitude);
            double latitude2 = DegreesToRadians(p2.Latitude);
            double longitudeDifference = DegreesToRadians(p2.Longitude - p1.Longitude);

            double y = Sin(longitudeDifference) * Cos(latitude2);
            double x =
                Cos(latitude1) * Sin(latitude2)
                - Sin(latitude1) * Cos(latitude2) * Cos(longitudeDifference);

            return (RadiansToDegrees(Atan2(y, x)) + 360) % 360;
        }

        /// <summary>
        ///     Conversion from cartesian earth-centered coordinates to geodetic coordinates in the given datum
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="height">Height above ellipsoid [m]</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void FromGeodeticToCartesian(
            double lat,
            double lng,
            double height,
            out double x,
            out double y,
            out double z
        )
        {
            lat = PI / 180 * lat;
            lng = PI / 180 * lng;

            double b = Axis * (1.0 - Flattening);
            double ee = 1.0 - b / Axis * (b / Axis);
            double n = Axis / Sqrt(1.0 - ee * Sin(lat) * Sin(lat));

            x = (n + height) * Cos(lat) * Cos(lng);
            y = (n + height) * Cos(lat) * Sin(lng);
            z = (n * (b / Axis) * (b / Axis) + height) * Sin(lat);
        }

        /// <summary>
        ///     Conversion from cartesian earth-sentered coordinates to geodetic coordinates in the given datum
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public void FromCartesianTGeodetic(
            double x,
            double y,
            double z,
            out double lat,
            out double lng
        )
        {
            double e = Flattening * (2.0 - Flattening);
            lng = Atan2(y, x);

            double p = Sqrt(x * x + y * y);
            double theta = Atan2(z, p * (1.0 - Flattening));
            double st = Sin(theta);
            double ct = Cos(theta);
            lat = Atan2(
                z + e / (1.0 - Flattening) * Axis * st * st * st,
                p - e * Axis * ct * ct * ct
            );

            lat /= PI / 180;
            lng /= PI / 180;
        }

        public static List<GeoPoint> PolylineDecode(string encodedPath)
        {
            var path = new List<GeoPoint>();

            // https://github.com/googlemaps/google-maps-services-java/blob/master/src/main/java/com/google/maps/internal/PolylineEncoding.java
            int len = encodedPath.Length;
            int index = 0;
            int lat = 0;
            int lng = 0;

            while (index < len)
            {
                int result = 1;
                int shift = 0;
                int b;

                do
                {
                    b = encodedPath[index++] - 63 - 1;
                    result += b << shift;
                    shift += 5;
                } while (b >= 0x1f && index < len);

                lat += (result & 1) != 0 ? ~(result >> 1) : result >> 1;

                result = 1;
                shift = 0;

                if (index < len)
                {
                    do
                    {
                        b = encodedPath[index++] - 63 - 1;
                        result += b << shift;
                        shift += 5;
                    } while (b >= 0x1f && index < len);

                    lng += (result & 1) != 0 ? ~(result >> 1) : result >> 1;
                }

                path.Add(new GeoPoint(lat * 1e-5, lng * 1e-5, 0));
            }

            return path;
        }

        public static void PolylineDecode(List<GeoPoint> path, string encodedPath)
        {
            path.AddRange(PolylineDecode(encodedPath));
        }

        public static String PolylineEncode(List<GeoPoint> path)
        {
            long lastLat = 0;
            long lastLng = 0;

            var result = new StringBuilder();

            foreach (var point in path)
            {
                long lat = Convert.ToInt64(Round(point.Latitude * 1e5));
                long lng = Convert.ToInt64(Round(point.Longitude * 1e5));

                long dLat = lat - lastLat;
                long dLng = lng - lastLng;

                Encode(dLat, result);
                Encode(dLng, result);

                lastLat = lat;
                lastLng = lng;
            }

            return result.ToString();
        }

        private static void Encode(long point, StringBuilder result)
        {
            point = point < 0 ? ~(point << 1) : point << 1;

            while (point >= 0x20)
            {
                result.Append(Convert.ToChar((int)((0x20 | (point & 0x1f)) + 63)));
                point >>= 5;
            }

            result.Append(Convert.ToChar((int)(point + 63)));
        }

        #endregion
    }
}
