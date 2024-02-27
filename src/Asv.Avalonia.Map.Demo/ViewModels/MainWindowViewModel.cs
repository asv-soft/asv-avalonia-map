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
using DynamicData.Binding;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.Map.Demo
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel()
        {
            this.WhenValueChanged(vm => vm.IsInAnchorEditMode).Subscribe(v =>
            {
                if (IsInAnchorEditMode)
                {
                    foreach (var item in _markers)
                    {
                        item.Stroke = Brushes.Firebrick;
                    }
                    
                }
            });
            CurrentMapProvider = GMapProviders.GoogleMap;
            SelectedAnchorVariant = AnchorViewModels[0];
            _markers = new ObservableCollection<MapAnchorViewModel>
            {
                new()
                {
                    IsEditable = true,
                    ZOrder = 0,
                    OffsetX = OffsetXEnum.Left,
                    OffsetY = OffsetYEnum.Top,
                    IsSelected = true,
                    IsVisible = true,
                    Icon = MaterialIconKind.Navigation,
                    Size = 32,
                    IconBrush = Brushes.LightSeaGreen,
                    Title = "Hello!!!",
                }
            };
            AddAnchor = ReactiveCommand.Create(AddNewAnchor);
            RemoveAllAnchorsCommand = ReactiveCommand.Create(RemoveAllAnchors);
        }

        #region Anchors Actions

        [Reactive] public bool IsInAnchorEditMode { get; set; }
        [Reactive] public ReactiveCommand<Unit, Unit> AddAnchor { get; set; }
        [Reactive] public ReactiveCommand<Unit, Unit> RemoveAllAnchorsCommand { get; set; }
        [Reactive] public MapAnchorViewModel SelectedAnchorVariant { get; set; }
        public IEnumerable<GMapProvider> AvailableProviders => GMapProviders.List;
        [Reactive] public GMapProvider CurrentMapProvider { get; set; }

        private void RemoveAllAnchors()
        {
            _markers.Clear();
        }

        private CancellationTokenSource _tokenSource = new();

        private async void AddNewAnchor()
        {
            await _tokenSource.CancelAsync();
            _tokenSource = new CancellationTokenSource();

            try
            {
                var userPoint = await ShowTargetDialog("Set a point", _tokenSource.Token);
                var newAnchor = new MapAnchorViewModel()
                {
                    IsEditable = SelectedAnchorVariant.IsEditable,
                    ZOrder = SelectedAnchorVariant.ZOrder,
                    OffsetX = SelectedAnchorVariant.OffsetX,
                    OffsetY = SelectedAnchorVariant.OffsetY,
                    IsSelected = SelectedAnchorVariant.IsSelected,
                    IsVisible = SelectedAnchorVariant.IsVisible,
                    Icon = SelectedAnchorVariant.Icon,
                    Size = SelectedAnchorVariant.Size,
                    IconBrush = SelectedAnchorVariant.IconBrush,
                    Title = SelectedAnchorVariant.Title,
                };
                newAnchor.Location = userPoint;
                _markers.Add(new ObservableCollection<MapAnchorViewModel>
                {
                    newAnchor
                });
            }
            catch (TaskCanceledException)
            {
            }
        }

        public List<MapAnchorViewModel> AnchorViewModels => new()
        {
            new()
            {
                IsEditable = true,
                ZOrder = 0,
                OffsetX = OffsetXEnum.Center,
                OffsetY = OffsetYEnum.Bottom,
                IsSelected = true,
                IsVisible = true,
                Icon = MaterialIconKind.MapMarker,
                Size = 32,
                IconBrush = Brushes.LightSeaGreen,
                Title = "Map Marker",
            },
            new()
            {
                IsEditable = true,
                ZOrder = 0,
                OffsetX = OffsetXEnum.Center,
                OffsetY = OffsetYEnum.Center,
                IsSelected = true,
                IsVisible = true,
                Icon = MaterialIconKind.Navigation,
                Size = 32,
                IconBrush = Brushes.LightSeaGreen,
                Title = "Vehicle",
            }
        };

        #endregion

        #region Map Properties

        public ObservableCollection<MapAnchorViewModel> Markers => _markers;

        public MapAnchorViewModel SelectedItem { get; set; }

        [Reactive] public GeoPoint Center { get; set; }
        [Reactive] public GeoPoint DialogTarget { get; set; }
        [Reactive] public bool IsInDialogMode { get; set; }
        [Reactive] public string DialogText { get; set; }

        private readonly ObservableCollection<MapAnchorViewModel> _markers;

        private async Task<GeoPoint> ShowTargetDialog(string text, CancellationToken cancel)
        {
            DialogText = text;
            IsInDialogMode = true;
            var tcs = new TaskCompletionSource();
            await using var c1 = cancel.Register(() => tcs.TrySetCanceled());
            this.WhenAnyValue(_ => _.IsInDialogMode).Where(_ => IsInDialogMode == false)
                .Subscribe(_ => tcs.TrySetResult(), cancel);
            await tcs.Task;
            return DialogTarget;
        }

        #endregion
    }
}