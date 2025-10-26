namespace PsychologyApp.Presentation.Templates;

public partial class AlgorithmBoxView : ContentView
{
    public AlgorithmBoxView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(AlgorithmBoxView), string.Empty, BindingMode.TwoWay);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly BindableProperty BodySourceProperty =
        BindableProperty.Create(nameof(BodySource), typeof(IEnumerable<string>), typeof(AlgorithmBoxView), default, BindingMode.TwoWay);

    public IEnumerable<string> BodySource
    {
        get => (IEnumerable<string>)GetValue(BodySourceProperty);
        set => SetValue(BodySourceProperty, value);
    }
}