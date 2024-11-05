﻿using System;

namespace Asv.Avalonia.Map
{
    /// <summary>
    ///     CzechHistoryMap provider, http://www.mapy.cz/
    /// </summary>
    public class CzechHistoryMapProviderOld : CzechMapProviderBaseOld
    {
        public static readonly CzechHistoryMapProviderOld Instance;

        CzechHistoryMapProviderOld() { }

        static CzechHistoryMapProviderOld()
        {
            Instance = new CzechHistoryMapProviderOld();
        }

        #region GMapProvider Members

        public override Guid Id { get; } = new Guid("C666AAF4-9D27-418F-97CB-7F0D8CC44544");

        public override string Name { get; } = "CzechHistoryOldMap";

        GMapProvider[] _overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (_overlays == null)
                {
                    _overlays = new GMapProvider[] { this, CzechHybridMapProviderOld.Instance };
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
            // http://m4.mapserver.mapy.cz/army2/9_7d00000_8080000

            long xx = pos.X << (28 - zoom);
            long yy = ((long)Math.Pow(2.0, zoom) - 1 - pos.Y) << (28 - zoom);

            return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, xx, yy);
        }

        static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/army2/{1}_{2:x7}_{3:x7}";
    }
}
