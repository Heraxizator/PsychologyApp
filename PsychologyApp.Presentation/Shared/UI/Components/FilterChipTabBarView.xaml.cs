using System.Collections;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class FilterChipTabBarView : ContentView
{
    private bool _themeSubscribed;

    public FilterChipTabBarView()
    {
        InitializeComponent();
        HandlerChanged += OnHandlerChanged;
    }

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(FilterChipTabBarView));

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(FilterChipTabBarView));

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
    {
        if (Handler is null)
        {
            UnsubscribeTheme();
        }
        else
        {
            SubscribeTheme();
        }
    }

    private void SubscribeTheme()
    {
        if (_themeSubscribed || Microsoft.Maui.Controls.Application.Current is null)
        {
            return;
        }

        Microsoft.Maui.Controls.Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
        _themeSubscribed = true;
    }

    private void UnsubscribeTheme()
    {
        if (!_themeSubscribed || Microsoft.Maui.Controls.Application.Current is null)
        {
            return;
        }

        Microsoft.Maui.Controls.Application.Current.RequestedThemeChanged -= OnRequestedThemeChanged;
        _themeSubscribed = false;
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        // CollectionView item templates often keep stale AppThemeBinding colors;
        // null → restore forces cell recreation with the current theme.
        IEnumerable? source = ItemsSource;
        if (source is null)
        {
            return;
        }

        ItemsSource = null;
        ItemsSource = source;
    }
}
