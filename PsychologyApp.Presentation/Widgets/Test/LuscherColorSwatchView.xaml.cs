namespace PsychologyApp.Presentation.Widgets.Test;

public partial class LuscherColorSwatchView : ContentView
{
    public LuscherColorSwatchView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty SwatchColorProperty =
        BindableProperty.Create(nameof(SwatchColor), typeof(Color), typeof(LuscherColorSwatchView), Colors.Transparent);

    public static readonly BindableProperty ColorNameProperty =
        BindableProperty.Create(nameof(ColorName), typeof(string), typeof(LuscherColorSwatchView), string.Empty);

    public static readonly BindableProperty SwatchSizeProperty =
        BindableProperty.Create(nameof(SwatchSize), typeof(double), typeof(LuscherColorSwatchView), 48d);

    public static readonly BindableProperty ShowNameProperty =
        BindableProperty.Create(nameof(ShowName), typeof(bool), typeof(LuscherColorSwatchView), true);

    public Color SwatchColor
    {
        get => (Color)GetValue(SwatchColorProperty);
        set => SetValue(SwatchColorProperty, value);
    }

    public string ColorName
    {
        get => (string)GetValue(ColorNameProperty);
        set => SetValue(ColorNameProperty, value);
    }

    public double SwatchSize
    {
        get => (double)GetValue(SwatchSizeProperty);
        set => SetValue(SwatchSizeProperty, value);
    }

    public bool ShowName
    {
        get => (bool)GetValue(ShowNameProperty);
        set => SetValue(ShowNameProperty, value);
    }

}
