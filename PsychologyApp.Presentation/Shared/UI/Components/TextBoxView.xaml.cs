namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class TextBoxView : ContentView
{
	public TextBoxView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty HeaderTextProperty =
        BindableProperty.Create(nameof(HeaderText), typeof(string), typeof(TextBoxView), string.Empty);

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public static readonly BindableProperty BodyTextProperty =
        BindableProperty.Create(nameof(BodyText), typeof(string), typeof(TextBoxView), string.Empty);

    public string BodyText
    {
        get => (string)GetValue(BodyTextProperty);
        set => SetValue(BodyTextProperty, value);
    }
}