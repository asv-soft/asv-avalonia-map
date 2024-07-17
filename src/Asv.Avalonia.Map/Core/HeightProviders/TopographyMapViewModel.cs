using System;

namespace Asv.Avalonia.Map.HeightProviders;

public class TopographyMapViewModel
{
    public SRTMProvider SrtmProvider  = new();
    public AsterProvider AsterProvider = new();
}