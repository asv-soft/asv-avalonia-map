﻿using System;

namespace Asv.Avalonia.Map
{
    /// <summary>
    ///     ArcGIS_World_Street_Map provider, http://server.arcgisonline.com
    /// </summary>
    public class ArcGIS_World_Street_MapProvider : ArcGISMapMercatorProviderBase
    {
        public static readonly ArcGIS_World_Street_MapProvider Instance;

        ArcGIS_World_Street_MapProvider()
        {
        }

        static ArcGIS_World_Street_MapProvider()
        {
            Instance = new ArcGIS_World_Street_MapProvider();
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get;
        } = new Guid("E1FACDF6-E535-4D69-A49F-12B623A467A9");

        public override string Name
        {
            get;
        } = "ArcGIS_World_Street_Map";

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom, LanguageStr);

            return GetTileImageUsingHttp(url);
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            // http://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/0/0/0jpg

            return string.Format(UrlFormat, zoom, pos.Y, pos.X);
        }

        static readonly string UrlFormat =
            "http://server.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{0}/{1}/{2}";
    }
}
