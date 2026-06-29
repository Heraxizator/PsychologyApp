namespace PsychologyApp.Presentation.Widgets.Test;

public partial class LuscherColorResultView : ContentView
{
    public LuscherColorResultView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty SectionLabelProperty =
        BindableProperty.Create(nameof(SectionLabel), typeof(string), typeof(LuscherColorResultView), string.Empty);

    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(nameof(Color), typeof(Color), typeof(LuscherColorResultView), Colors.Transparent);

    public static readonly BindableProperty ColorNameProperty =
        BindableProperty.Create(nameof(ColorName), typeof(string), typeof(LuscherColorResultView), string.Empty);

    public static readonly BindableProperty ResultTextProperty =
        BindableProperty.Create(nameof(ResultText), typeof(string), typeof(LuscherColorResultView), string.Empty);

    public string SectionLabel
    {
        get => (string)GetValue(SectionLabelProperty);
        set => SetValue(SectionLabelProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public string ColorName
    {
        get => (string)GetValue(ColorNameProperty);
        set => SetValue(ColorNameProperty, value);
    }

    public string ResultText
    {
        get => (string)GetValue(ResultTextProperty);
        set => SetValue(ResultTextProperty, value);
    }
}
