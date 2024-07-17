using System.Collections.ObjectModel;

namespace Asv.Avalonia.Map.HeightProviders;

public class GetOpenTopoDataResponseModel(ObservableCollection<Result> results)
{
    public ObservableCollection<Result> Results { get; init; } = results;
    public  string? Status { get; init; }
}
public class Result
{
  public string? Dataset { get; init; }
  public double? Elevation { get; init; }
  public class Locations 
  {
    public double Latitude { get; init; }
    public double Longitude { get; init; }
  }
}