using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class TextEditorView : ContentView
{
    public TextEditorView() => InitializeComponent();

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

    private void OnInputFocused(object? sender, FocusEventArgs e) =>
        InputFieldChrome.ApplyFocusAsync(InputBorder, Variant).FireAndForget();

    private void OnInputUnfocused(object? sender, FocusEventArgs e) =>
        InputFieldChrome.ApplyBlurAsync(InputBorder, Variant).FireAndForget();
}
