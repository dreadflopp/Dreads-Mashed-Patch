using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using DreadsMashedPatch.App.Models;

namespace DreadsMashedPatch.App;

public partial class App : Application
{
    private AppTheme _theme = AppTheme.Light;

    static App()
    {
        EventManager.RegisterClassHandler(typeof(Window), Keyboard.KeyDownEvent,
            new KeyEventHandler(OnUnhandledNavigationKeyDown));
        EventManager.RegisterClassHandler(typeof(ScrollViewer), Keyboard.KeyDownEvent,
            new KeyEventHandler(OnUnhandledNavigationKeyDown));
    }

    private static void OnUnhandledNavigationKeyDown(object sender, KeyEventArgs e)
    {
        // Let editors and item controls process keys routed through their template's scroller.
        if (sender is ScrollViewer { TemplatedParent: TextBoxBase or ItemsControl })
        {
            return;
        }

        // Consume remaining arrows before page scrollers or WPF move focus spatially.
        if (e.Key is Key.Left or Key.Right or Key.Up or Key.Down)
        {
            e.Handled = true;
        }
    }

    public void SetTheme(AppTheme theme)
    {
        _theme = theme;
        ApplyPalette();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        ApplyPalette();
        SystemParameters.StaticPropertyChanged += OnSystemParametersChanged;
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SystemParameters.StaticPropertyChanged -= OnSystemParametersChanged;
        base.OnExit(e);
    }

    private void OnSystemParametersChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SystemParameters.HighContrast))
        {
            Dispatcher.InvokeAsync(ApplyPalette);
        }
    }

    private void ApplyPalette()
    {
        // The first dictionary supplies colors; control templates stay shared on both platforms.
        var palette = SystemParameters.HighContrast ? "HighContrast"
            : _theme == AppTheme.Dark ? "DarkTheme" : "LightTheme";
        Resources.MergedDictionaries[0] = new ResourceDictionary
        {
            Source = new Uri($"Styles/{palette}.xaml", UriKind.Relative)
        };
    }

    private static void OnDispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            e.Exception.ToString(),
            "Mashed Patch encountered an unexpected error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }
}
