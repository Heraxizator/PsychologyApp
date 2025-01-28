namespace PsychologyApp.Presentation.Templates;

public partial class NavigationBarSimpleView : ContentView
{
	public NavigationBarSimpleView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(NavigationBarSimpleView), string.Empty);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly BindableProperty BackTextProperty =
        BindableProperty.Create(nameof(BackText), typeof(string), typeof(NavigationBarSimpleView), string.Empty);

    public string BackText
    {
        get => (string)GetValue(BackTextProperty);
        set => SetValue(BackTextProperty, value);
    }

    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(nameof(BackCommand), typeof(Command), typeof(NavigationBarSimpleView), default, BindingMode.TwoWay);

    public Command BackCommand
    {
        get => (Command)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }
}