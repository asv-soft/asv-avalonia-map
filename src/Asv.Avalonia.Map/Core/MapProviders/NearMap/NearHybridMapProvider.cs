﻿using System;

namespace Asv.Avalonia.Map
{
    /// <summary>
    ///     NearHybridMap provider - http://www.nearmap.com/
    /// </summary>
    public class NearHybridMapProvider : NearMapProviderBase
    {
        public static readonly NearHybridMapProvider Instance;

        NearHybridMapProvider() { }

        static NearHybridMapProvider()
        {
            Instance = new NearHybridMapProvider();
        }

        #region GMapProvider Members

        public override Guid Id { get; } = new Guid("4BF8819A-635D-4A94-8DC7-94C0E0F04BFD");

        public override string Name { get; } = "NearHybridMap";

        GMapProvider[] _overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (_overlays == null)
                {
                    _overlays = new GMapProvider[] { NearSatelliteMapProvider.Instance, this };
                }

                return _overlays;
            }
        }

        public override PureImage? GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom, LanguageStr);

            return GetTileImageUsingHttp(url);
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            // http://web1.nearmap.com/maps/hl=en&x=37&y=19&z=6&nml=MapT&nmg=1&s=2KbhmZZ
            // http://web1.nearmap.com/maps/hl=en&x=36&y=19&z=6&nml=MapT&nmg=1&s=2YKWhQi
            return string.Format(UrlFormat, GetServerNum(pos, 3), pos.X, pos.Y, zoom);
        }

        static readonly string UrlFormat =
            "http://web{0}.nearmap.com/maps/hl=en&x={1}&y={2}&z={3}&nml=MapT&nmg=1";
    }
}
