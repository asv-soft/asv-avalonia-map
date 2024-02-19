using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace Asv.Avalonia.Map.Demo.Menu.Actions.AnchorMover;

//[ExportView(typeof(AnchorMoverActionViewModel))]
public partial class AnchorMoverActionView : ReactiveUserControl<AnchorMoverActionViewModel>
{
    //[ImportingConstructor]
    public AnchorMoverActionView()
    {
        InitializeComponent();
        HotKeyManager.SetHotKey(editAnchorsToggleButton, new KeyGesture(Key.LeftAlt, KeyModifiers.Alt));
    }
}