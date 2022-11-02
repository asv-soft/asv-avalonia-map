﻿using System.Collections.Generic;
using Asv.Common;

namespace Asv.Avalonia.Map
{
    /// <summary>
    ///     directions interface
    /// </summary>
    public interface DirectionsProvider
    {
        DirectionsStatusCode GetDirections(out GDirections direction, GeoPoint start, GeoPoint end,
            bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric);

        DirectionsStatusCode GetDirections(out GDirections direction, string start, string end, bool avoidHighways,
            bool avoidTolls, bool walkingMode, bool sensor, bool metric);

        /// <summary>
        ///     service may provide more than one route alternative in the response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="avoidHighways"></param>
        /// <param name="avoidTolls"></param>
        /// <param name="walkingMode"></param>
        /// <param name="sensor"></param>
        /// <param name="metric"></param>
        /// <returns></returns>
        IEnumerable<GDirections> GetDirections(out DirectionsStatusCode status, string start, string end,
            bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric);

        /// <summary>
        ///     service may provide more than one route alternative in the response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="avoidHighways"></param>
        /// <param name="avoidTolls"></param>
        /// <param name="walkingMode"></param>
        /// <param name="sensor"></param>
        /// <param name="metric"></param>
        /// <returns></returns>
        IEnumerable<GDirections> GetDirections(out DirectionsStatusCode status, GeoPoint start, GeoPoint end,
            bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric);

        DirectionsStatusCode GetDirections(out GDirections direction, GeoPoint start,
            IEnumerable<GeoPoint> wayPoints, GeoPoint end, bool avoidHighways, bool avoidTolls, bool walkingMode,
            bool sensor, bool metric);

        DirectionsStatusCode GetDirections(out GDirections direction, string start, IEnumerable<string> wayPoints,
            string end, bool avoidHighways, bool avoidTolls, bool walkingMode, bool sensor, bool metric);
    }
}
