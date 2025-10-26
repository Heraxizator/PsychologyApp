using System.Windows.Input;

namespace PsychologyApp.Presentation.Templates;

public partial class TextEntryView : ContentView
{
	public TextEntryView()
	{
		InitializeComponent();
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
}