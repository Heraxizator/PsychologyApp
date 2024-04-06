namespace PsychologyApp.Presentation.Templates;

public partial class RetryView : ContentView
{
	public RetryView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty FailedTextProperty =
        BindableProperty.Create(nameof(FailedText), typeof(string), typeof(RetryView), string.Empty);

    public string FailedText
    {
        get => (string)GetValue(FailedTextProperty);
        set => SetValue(FailedTextProperty, value);
    }

    public static readonly BindableProperty RetryTextProperty =
        BindableProperty.Create(nameof(RetryText), typeof(string), typeof(RetryView), string.Empty);

    public string RetryText
    {
        get => (string) GetValue(RetryTextProperty);
        set => SetValue(RetryTextProperty, value);
    }

    public static readonly BindableProperty RetryCommandProperty =
        BindableProperty.Create(nameof(RetryCommand), typeof(Command), typeof(RetryView), null);

    public Command RetryCommand
    {
        get => (Command)GetValue(RetryCommandProperty);
        set => SetValue(RetryTextProperty, value);
    }
}