using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class TextEntryView : ContentView
{
    private bool _themeSubscribed;

    public TextEntryView()
    {
        InitializeComponent();
        HandlerChanged += OnHandlerChanged;
    }

    public static readonly BindableProperty VariantProperty =
        BindableProperty.Create(nameof(Variant), typeof(string), typeof(TextEntryView), InputFieldChrome.VariantDefault);

    public string Variant
    {
        get => (string)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(TextEntryView), string.Empty);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty =
        BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(TextEntryView), string.Empty);

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty BodyTextProperty =
        BindableProperty.Create(nameof(BodyText), typeof(string), typeof(TextEntryView), string.Empty, BindingMode.TwoWay);

    public string BodyText
    {
        get => (string)GetValue(BodyTextProperty);
        set => SetValue(BodyTextProperty, value);
    }

    public static readonly BindableProperty ChangedCommandProperty =
        BindableProperty.Create(nameof(ChangedCommand), typeof(ICommand), typeof(TextEntryView), default, BindingMode.TwoWay);

    public ICommand ChangedCommand
    {
        get => (ICommand)GetValue(ChangedCommandProperty);
        set => SetValue(ChangedCommandProperty, value);
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

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e) =>
        InputFieldChrome.RefreshForThemeAsync(InputBorder, Variant, InputEntry.IsFocused).FireAndForget();

    private void OnInputFocused(object? sender, FocusEventArgs e) =>
        InputFieldChrome.ApplyFocusAsync(InputBorder, Variant).FireAndForget();

    private void OnInputUnfocused(object? sender, FocusEventArgs e) =>
        InputFieldChrome.ApplyBlurAsync(InputBorder, Variant).FireAndForget();

    private void OnInputTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (ChangedCommand?.CanExecute(e.NewTextValue) == true)
        {
            ChangedCommand.Execute(e.NewTextValue);
        }
    }
}
