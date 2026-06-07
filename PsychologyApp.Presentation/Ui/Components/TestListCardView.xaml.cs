using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class TestListCardView : ContentView
{
    public TestListCardView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TestListCardView), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(TestListCardView), string.Empty);

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly BindableProperty LastResultTextProperty =
        BindableProperty.Create(nameof(LastResultText), typeof(string), typeof(TestListCardView), string.Empty);

    public string LastResultText
    {
        get => (string)GetValue(LastResultTextProperty);
        set => SetValue(LastResultTextProperty, value);
    }

    public static readonly BindableProperty HasLastResultProperty =
        BindableProperty.Create(nameof(HasLastResult), typeof(bool), typeof(TestListCardView), false);

    public bool HasLastResult
    {
        get => (bool)GetValue(HasLastResultProperty);
        set => SetValue(HasLastResultProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(TestListCardView), null);

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }
}
