namespace PsychologyApp.Presentation.Templates;

public partial class ProgressbarView : ContentView
{
	public ProgressbarView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty IsLoadingProperty =
        BindableProperty.Create(nameof(IsLoading), typeof(bool), typeof(ProgressbarView), false);

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public static readonly BindableProperty ProgressTextProperty =
        BindableProperty.Create(nameof(ProgressText), typeof(string), typeof(ProgressbarView), string.Empty);

    public string ProgressText
    {
        get => (string)GetValue(ProgressTextProperty);
        set => SetValue(ProgressTextProperty, value);
    }

    public static readonly BindableProperty CancelCommandProperty =
        BindableProperty.Create(nameof(CancelCommand), typeof(Command), typeof(ProgressbarView), null);

    public Command CancelCommand
    {
        get => (Command)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }
}