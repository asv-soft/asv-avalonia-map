using System;
using System.Threading.Tasks;
using DynamicData;
using Material.Icons;

namespace Asv.Avalonia.Map.Demo.Models;

public interface IShellPage : IViewModel
{
    MaterialIconKind Icon { get; }
    string Title { get; }
}