using System;
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
        private CancellationTokenSource _tokenSource = new();

        private readonly ObservableCollection<MapAnchorViewModel> _markers;

        public MainWindowViewModel()
        {
            _markers = new ObservableCollection<MapAnchorViewModel>
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
                    Size = 32,
                    IconBrush = Brushes.LightSeaGreen,
                    Title = "Hello!!!",
                }
            };
            AddAnchor = ReactiveCommand.Create(AddNewAnchor);
            RemoveAllAnchorsCommand = ReactiveCommand.Create(RemoveAllAnchors);
        }

        [Reactive] public GeoPoint Center { get; set; }

        public ObservableCollection<MapAnchorViewModel> Markers => _markers;

        private MapAnchorViewModel _selectedItem;

        public MapAnchorViewModel SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        private GeoPoint _dialogTarget;

        [Reactive]
        public GeoPoint DialogTarget
        {
            get => _dialogTarget;
            set => this.RaiseAndSetIfChanged(ref _dialogTarget, value);
        }

        private bool _isInDialogMode;

        [Reactive]
        public bool IsInDialogMode
        {
            get => _isInDialogMode;
            set => this.RaiseAndSetIfChanged(ref _isInDialogMode, value);
        }

        private string _dialogText;

        [Reactive]
        public string DialogText
        {
            get => _dialogText;
            set => this.RaiseAndSetIfChanged(ref _dialogText, value);
        }

        [Reactive] public ReactiveCommand<Unit, Unit> AddAnchor { get; set; }
        [Reactive] public ReactiveCommand<Unit, Unit> RemoveAllAnchorsCommand { get; set; }

        private void RemoveAllAnchors()
        {
            _markers.Clear();
        }

        private async void AddNewAnchor()
        {
            await _tokenSource.CancelAsync();
            _tokenSource = new CancellationTokenSource();

            try
            {
                var userPoint = await ShowTargetDialog("Set a point", _tokenSource.Token);

                var newAnchor = new MapAnchorViewModel
                {
                    IsEditable = true,
                    ZOrder = 0,
                    OffsetX = OffsetXEnum.Center,
                    OffsetY = OffsetYEnum.Center,
                    IsSelected = true,
                    IsVisible = true,
                    Icon = MaterialIconKind.Aeroplane,
                    Size = 32,
                    IconBrush = Brushes.LightSeaGreen,
                    Title = "Hello!!!",
                    Location = userPoint
                };

                _markers.Add(new ObservableCollection<MapAnchorViewModel>
                {
                    newAnchor
                });
            }
            catch (TaskCanceledException)
            {
            }
        }

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
    }
}