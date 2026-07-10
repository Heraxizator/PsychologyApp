using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class TextEditorView : ContentView
{
    private bool _themeSubscribed;

    public TextEditorView()
    {
        InitializeComponent();
        HandlerChanged += OnHandlerChanged;
    }

    public static readonly BindableProperty VariantProperty =
        BindableProperty.Create(nameof(Variant), typeof(string), typeof(TextEditorView), InputFieldChrome.VariantDefault);

    public string Variant
    {
        get => (string)GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(TextEditorView), string.Empty);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly BindableProperty PlaceholderTextProperty =
        BindableProperty.Create(nameof(PlaceholderText), typeof(string), typeof(TextEditorView), string.Empty);

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty BodyTextProperty =
        BindableProperty.Create(nameof(BodyText), typeof(string), typeof(TextEditorView), string.Empty, BindingMode.TwoWay);

    public string BodyText
    {
        get => (string)GetValue(BodyTextProperty);
        set => SetValue(BodyTextProperty, value);
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
        InputFieldChrome.RefreshForThemeAsync(InputBorder, Variant, InputEditor.IsFocused).FireAndForget();

    private void OnInputFocused(object? sender, FocusEventArgs e) =>
        InputFieldChrome.ApplyFocusAsync(InputBorder, Variant).FireAndForget();

    private void OnInputUnfocused(object? sender, FocusEventArgs e) =>
        InputFieldChrome.ApplyBlurAsync(InputBorder, Variant).FireAndForget();
}
