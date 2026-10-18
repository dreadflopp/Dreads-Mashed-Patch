using System.Windows;
using System.Windows.Input;

namespace DreadsMashedPatch.App.Styles;

// One shared state for focus adorners, including those in dropdown popups.
// WPF still controls focus traversal and when an adorner is created.
public sealed class FocusVisualState : DependencyObject
{
    private static readonly DependencyPropertyKey IsVisiblePropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(IsVisible), typeof(bool), typeof(FocusVisualState),
            new PropertyMetadata(false));

    public static readonly DependencyProperty IsVisibleProperty = IsVisiblePropertyKey.DependencyProperty;

    private bool _keyboardInput;

    public FocusVisualState()
    {
        _keyboardInput = InputManager.Current.MostRecentInputDevice is KeyboardDevice;
        InputManager.Current.PreProcessInput += OnPreProcessInput;
        UpdateVisibility();
    }

    public bool IsVisible => (bool)GetValue(IsVisibleProperty);

    private void OnPreProcessInput(object sender, PreProcessInputEventArgs e)
    {
        switch (e.StagingItem.Input)
        {
            case KeyEventArgs { RoutedEvent: var routedEvent } when routedEvent == Keyboard.PreviewKeyDownEvent:
                _keyboardInput = true;
                break;
            case MouseButtonEventArgs { RoutedEvent: var routedEvent } when routedEvent == Mouse.PreviewMouseDownEvent:
                _keyboardInput = false;
                break;
            default:
                return;
        }

        UpdateVisibility();
    }

    // Mouse input must hide the outline even when Windows always shows keyboard cues.
    private void UpdateVisibility() =>
        SetValue(IsVisiblePropertyKey, _keyboardInput);
}
