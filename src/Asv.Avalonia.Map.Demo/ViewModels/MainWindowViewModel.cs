using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Avalonia.Media;
using DynamicData;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.Map.Demo
{
    public class MainWindowViewModel : ReactiveObject
    {

        private int _anchorindex = 0;
        private MapAnchorViewModel _selectedItem;
        private readonly ObservableCollection<MapAnchorViewModel> _markers;
        private readonly ObservableCollection<MapAnchorViewModel> _markersVariantCollection;
        

        public MainWindowViewModel()
        {
            
            _markers =  new ObservableCollection<MapAnchorViewModel>
            {
                
                new MapAnchorViewModel
                {
                    IsEditable = true,
                    ZOrder = 0,
                    OffsetX = OffsetXEnum.Left,
                    OffsetY = OffsetYEnum.Top,
                    IsSelected = true,
                    IsVisible = true,
                    Icon = MaterialIconKind.Navigation,
                    Size=32,
                    IconBrush = Brushes.LightSeaGreen,
                    Title="Hello!!!",
                }
                
            };

            // _markersVariantCollection = new ObservableCollection<MapAnchorViewModel>
            // {
            //   new ()
            //   {
            //       IsEditable = true,
            //       ZOrder = 0,
            //       OffsetX = OffsetXEnum.Left,
            //       OffsetY = OffsetYEnum.Top,
            //       IsSelected = true,
            //       IsVisible = true,
            //       Icon = MaterialIconKind.Navigation,
            //       Size=32,
            //       IconBrush = Brushes.LightSeaGreen,
            //       Title="Vehicle",
            //   },
            //   new ()
            //   {
            //       IsEditable = true,
            //       ZOrder = 0,
            //       OffsetX = OffsetXEnum.Left,
            //       OffsetY = OffsetYEnum.Top,
            //       IsSelected = true,
            //       IsVisible = true,
            //       Icon = MaterialIconKind.MapMarker,
            //       Size=32,
            //       IconBrush = Brushes.LightSeaGreen,
            //       Title="Marker",
            //   }
            //   
            // };   /// Template for anchors type selection feature
            AddAnchor = ReactiveCommand.Create(AddNewAnchor);
            RemoveAllAnchorsCommand = ReactiveCommand.Create(RemoveAllAnchors);
        }

        [Reactive]
        public double Zoom { get; set; } = 7;
        [Reactive]
        public GeoPoint Center { get; set; }

        public ObservableCollection<MapAnchorViewModel> Markers => _markers;
        public ObservableCollection<MapAnchorViewModel> MarkersVariantCollection => _markersVariantCollection;

        public MapAnchorViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedItem, value);
            }
        }

        [Reactive]
        public int MinZoom { get; set; } = 1;
        [Reactive]
        public int MaxZoom { get; set; } = 20;

        private GeoPoint _dialogtarget;
        [Reactive]
        public GeoPoint DialogTarget { 
            get =>_dialogtarget;
            set
            {
                this.RaiseAndSetIfChanged(ref _dialogtarget, value);
            }
        }
    
         
        private bool _isInDialogMode;
        [Reactive]
        public bool IsInDialogMode { 
            get=> _isInDialogMode;
            set
            {
                this.RaiseAndSetIfChanged(ref _isInDialogMode, value);
            } 
        }

        private string _dialogText;
        [Reactive]
        public string DialogText { 
            get=>_dialogText;
            set
            {
                this.RaiseAndSetIfChanged(ref _dialogText, value);
            } 
        }


        private CancellationTokenSource _tokenSource = new ();
        [Reactive]
        public ReactiveCommand<Unit,Unit> AddAnchor { get; set; }
        [Reactive]
        public ReactiveCommand<Unit,Unit> RemoveAllAnchorsCommand { get; set; }

        public void RemoveAllAnchors()
        {
            _markers.Clear();
        }

        public async void AddNewAnchor()
        {
           
            _tokenSource.Cancel();
            _tokenSource = new();
            try
            {
                var userpoint= await ShowTargetDialog("Set a point", _tokenSource.Token);
                
                MapAnchorViewModel newAnchor = new MapAnchorViewModel()
                {
                    IsEditable = true,
                    ZOrder = 0,
                    OffsetX = OffsetXEnum.Center,
                    OffsetY = OffsetYEnum.Center,
                    IsSelected = true,
                    IsVisible = true,
                    Icon = MaterialIconKind.Aeroplane,
                    Size=32,
                    IconBrush = Brushes.LightSeaGreen,
                    Title="Hello!!!",
                    Location = userpoint
                };
                _markers.Add( new ObservableCollection<MapAnchorViewModel>()
                {
                    newAnchor
                });
            }
            catch (TaskCanceledException)
            {
                
            }
           
            return;
        }

        public async Task<GeoPoint> ShowTargetDialog(string text, CancellationToken cancel)
        {
            DialogText = text;
            IsInDialogMode = true;
            var tcs = new TaskCompletionSource();
            await using var c1 = cancel.Register(() => tcs.TrySetCanceled());
            this.WhenAnyValue(_ => _.IsInDialogMode).Where(_ => IsInDialogMode == false).Subscribe(_ => tcs.TrySetResult(), cancel);
            await tcs.Task;
            return DialogTarget;
        }

        private MapAnchorViewModel anchorViewModel = new MapAnchorViewModel()
        {
            IsEditable = true,
            ZOrder = 0,
            OffsetX = OffsetXEnum.Center,
            OffsetY = OffsetYEnum.Center,
            IsSelected = true,
            IsVisible = true,
            Size=32,
            IconBrush = Brushes.IndianRed,
            Title="Hello!!!",
        };

    }
}
