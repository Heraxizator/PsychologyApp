namespace PsychologyApp.Presentation.Templates;

public partial class ButtonView : ContentView
{
    public ButtonView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty BodyTextProperty =
        BindableProperty.Create(nameof(BodyText), typeof(string), typeof(RetryView), string.Empty, BindingMode.TwoWay);

    public string BodyText
    {
        get => (string)GetValue(BodyTextProperty);
        set => SetValue(BodyTextProperty, value);
    }
}