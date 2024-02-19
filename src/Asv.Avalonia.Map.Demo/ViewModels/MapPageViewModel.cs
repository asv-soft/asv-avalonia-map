using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Avalonia.Map.Demo.Menu;
using Asv.Avalonia.Map.Demo.Models;
using Asv.Common;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.Map.Demo;

public class MapPageViewModel: ShellPage, IMap
{
    public static string Uri = "asv:shell.page.map";
    public MapPageViewModel():base("asv:shell.page.map")
    {
            
    }
   
    public MapPageViewModel(Uri id) : base(id)
    {
    }

    public MapPageViewModel(string id) : base(id)
    {
    }

   
    public int MaxZoom { get; set; }
    public int MinZoom { get; set; }
    public double Zoom { get; set; }
    public GeoPoint Center { get; set; }
    public ReadOnlyObservableCollection<IMapAnchor> Markers { get; }
    public IMapAnchor SelectedItem { get; set; }
    public IMapAnchor? ItemToFollow { get; set; }
    public bool IsInAnchorEditMode { get; set; }
    
    [Reactive]
    public GeoPoint DialogTarget { get; set; }
    [Reactive]
    public bool IsInDialogMode { get; set; }
    [Reactive]
    public string DialogText { get; set; }
    public async Task<GeoPoint> ShowTargetDialog(string text, CancellationToken cancel)
    {
        var tcs = new TaskCompletionSource<GeoPoint>();
        DialogText = text;
        IsInDialogMode = true;
            
        await using var c1 = cancel.Register(() =>
        {
            tcs.TrySetCanceled();
            IsInDialogMode = false;
            SelectedItem = null;
        });
            
        this.WhenAnyValue(_ => _.IsInDialogMode).Where(_ => IsInDialogMode == false).Subscribe(_ =>
        {
            if (!tcs.Task.IsCanceled)
            {
                tcs.TrySetResult(DialogTarget);
            }
        }, cancel);
            
        await tcs.Task;
        return DialogTarget;
    }
}