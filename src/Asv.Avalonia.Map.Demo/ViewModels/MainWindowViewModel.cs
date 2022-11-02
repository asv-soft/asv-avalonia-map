using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.Common;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Asv.Avalonia.Map.Demo
{
    public class MainWindowViewModel : ReactiveObject
    {
        private MapAnchorViewModel _selectedItem;
        private readonly ReadOnlyObservableCollection<MapAnchorViewModel> _markers;

        public MainWindowViewModel()
        {
            _markers = new ReadOnlyObservableCollection<MapAnchorViewModel>(new ObservableCollection<MapAnchorViewModel>
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
            });

        }

        [Reactive]
        public double Zoom { get; set; } = 7;
        [Reactive]
        public GeoPoint Center { get; set; }

        public ReadOnlyObservableCollection<MapAnchorViewModel> Markers => _markers;

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
        [Reactive]
        public GeoPoint DialogTarget { get; set; }
        [Reactive]
        public bool IsInDialogMode { get; set; }
        [Reactive]
        public string DialogText { get; set; }

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
    }
}
