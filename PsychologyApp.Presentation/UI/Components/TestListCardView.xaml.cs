using PsychologyApp.Presentation.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class TestListCardView : ContentView
{
    public TestListCardView()
    {
        InitializeComponent();
        VisualElementPressFeedback.AttachToTemplateRoot(this, new PressFeedbackOptions { HapticOnRelease = true });
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

    public static readonly BindableProperty OpenHistoryCommandProperty =
        BindableProperty.Create(nameof(OpenHistoryCommand), typeof(ICommand), typeof(TestListCardView), null);

    public ICommand? OpenHistoryCommand
    {
        get => (ICommand?)GetValue(OpenHistoryCommandProperty);
        set => SetValue(OpenHistoryCommandProperty, value);
    }

    public static readonly BindableProperty HasMultipleResultsProperty =
        BindableProperty.Create(nameof(HasMultipleResults), typeof(bool), typeof(TestListCardView), false);

    public bool HasMultipleResults
    {
        get => (bool)GetValue(HasMultipleResultsProperty);
        set => SetValue(HasMultipleResultsProperty, value);
    }

    public static readonly BindableProperty HistoryLabelProperty =
        BindableProperty.Create(nameof(HistoryLabel), typeof(string), typeof(TestListCardView), string.Empty);

    public string HistoryLabel
    {
        get => (string)GetValue(HistoryLabelProperty);
        set => SetValue(HistoryLabelProperty, value);
    }
}
