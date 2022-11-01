﻿using System;

namespace Asv.Avalonia.Map
{
    public abstract class ArcGISMapPlateCarreeProviderBase : GMapProvider
    {
        public ArcGISMapPlateCarreeProviderBase()
        {
            Copyright = string.Format("©{0} ESRI - Map data ©{0} ArcGIS", DateTime.Today.Year);
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override PureProjection Projection
        {
            get
            {
                return PlateCarreeProjection.Instance;
            }
        }

        GMapProvider[] _overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (_overlays == null)
                {
                    _overlays = new GMapProvider[] {this};
                }

                return _overlays;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public abstract class ArcGISMapMercatorProviderBase : GMapProvider
    {
        public ArcGISMapMercatorProviderBase()
        {
            MaxZoom = null;
            Copyright = string.Format("©{0} ESRI - Map data ©{0} ArcGIS", DateTime.Today.Year);
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override PureProjection Projection
        {
            get
            {
                return MercatorProjection.Instance;
            }
        }

        GMapProvider[] _overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (_overlays == null)
                {
                    _overlays = new GMapProvider[] {this};
                }

                return _overlays;
            }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    ///     ArcGIS_StreetMap_World_2D_Map provider, http://server.arcgisonline.com
    /// </summary>
    public class ArcGIS_StreetMap_World_2D_MapProvider : ArcGISMapPlateCarreeProviderBase
    {
        public static readonly ArcGIS_StreetMap_World_2D_MapProvider Instance;

        ArcGIS_StreetMap_World_2D_MapProvider()
        {
        }

        static ArcGIS_StreetMap_World_2D_MapProvider()
        {
            Instance = new ArcGIS_StreetMap_World_2D_MapProvider();
        }

        #region GMapProvider Members

        public override Guid Id
        {
            get;
        } = new Guid("00BF56D4-4B48-4939-9B11-575BBBE4A718");

        public override string Name
        {
            get;
        } = "ArcGIS_StreetMap_World_2D_Map";

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            string url = MakeTileImageUrl(pos, zoom, LanguageStr);

            return GetTileImageUsingHttp(url);
        }

        #endregion

        string MakeTileImageUrl(GPoint pos, int zoom, string language)
        {
            // http://server.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer/tile/0/0/0.jpg

            return string.Format(UrlFormat, zoom, pos.Y, pos.X);
        }

        static readonly string UrlFormat =
            "http://server.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer/tile/{0}/{1}/{2}";
    }
}
