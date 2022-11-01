﻿using System;
using Asv.Gnss;

namespace Asv.Avalonia.Map
{
    /// <summary>
    ///     The Mercator projection
    ///     PROJCS["World_Mercator",GEOGCS["GCS_WGS_1984",DATUM["D_WGS_1984",SPHEROID["WGS_1984",6378137,298.257223563]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],PROJECTION["Mercator"],PARAMETER["False_Easting",0],PARAMETER["False_Northing",0],PARAMETER["Central_Meridian",0],PARAMETER["standard_parallel_1",0],UNIT["Meter",1]]
    /// </summary>
    public class MercatorProjection : PureProjection
    {
        public static readonly MercatorProjection Instance = new MercatorProjection();

        static readonly double MinLatitude = -85.05112878;
        static readonly double MaxLatitude = 85.05112878;
        static readonly double MinLongitude = -180;
        static readonly double MaxLongitude = 180;

        public override RectLatLng Bounds
        {
            get
            {
                return RectLatLng.FromLTRB(MinLongitude, MaxLatitude, MaxLongitude, MinLatitude);
            }
        }

        public override GSize TileSize
        {
            get;
        } = new GSize(256, 256);

        public override double Axis
        {
            get
            {
                return 6378137;
            }
        }

        public override double Flattening
        {
            get
            {
                return 1.0 / 298.257223563;
            }
        }

        public override GPoint FromLatLngToPixel(double lat, double lng, int zoom)
        {
            var ret = GPoint.Empty;

            lat = Clip(lat, MinLatitude, MaxLatitude);
            lng = Clip(lng, MinLongitude, MaxLongitude);

            double x = (lng + 180) / 360;
            double sinLatitude = Math.Sin(lat * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            var s = GetTileMatrixSizePixel(zoom);
            long mapSizeX = s.Width;
            long mapSizeY = s.Height;

            ret.X = (long)Clip(x * mapSizeX + 0.5, 0, mapSizeX - 1);
            ret.Y = (long)Clip(y * mapSizeY + 0.5, 0, mapSizeY - 1);

            return ret;
        }

        public override GeoPoint FromPixelToLatLng(long x, long y, int zoom)
        {
            var s = GetTileMatrixSizePixel(zoom);
            double mapSizeX = s.Width;
            double mapSizeY = s.Height;

            double xx = Clip(x, 0, mapSizeX - 1) / mapSizeX - 0.5;
            double yy = 0.5 - Clip(y, 0, mapSizeY - 1) / mapSizeY;

            return new GeoPoint(90 - 360 * Math.Atan(Math.Exp(-yy * 2 * Math.PI)) / Math.PI, 360 * xx,0);
        }

        public override GSize GetTileMatrixMinXY(int zoom)
        {
            return new GSize(0, 0);
        }

        public override GSize GetTileMatrixMaxXY(int zoom)
        {
            long xy = 1 << zoom;
            return new GSize(xy - 1, xy - 1);
        }
    }
}
