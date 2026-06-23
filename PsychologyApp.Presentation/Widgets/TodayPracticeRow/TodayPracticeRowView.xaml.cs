using PsychologyApp.Presentation.Shared.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.TodayPracticeRow;

public partial class TodayPracticeRowView : ContentView
{
    public TodayPracticeRowView()
    {
        InitializeComponent();
        VisualElementPressFeedback.AttachToTemplateRoot(this, new PressFeedbackOptions { HapticOnRelease = true, ScaleOnly = true });
    }

    public static readonly BindableProperty CaptionTextProperty =
        BindableProperty.Create(nameof(CaptionText), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public string CaptionText
    {
        get => (string)GetValue(CaptionTextProperty);
        set => SetValue(CaptionTextProperty, value);
    }

    public static readonly BindableProperty StreakTextProperty =
        BindableProperty.Create(nameof(StreakText), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public string StreakText
    {
        get => (string)GetValue(StreakTextProperty);
        set => SetValue(StreakTextProperty, value);
    }

    public static readonly BindableProperty HasStreakProperty =
        BindableProperty.Create(nameof(HasStreak), typeof(bool), typeof(TodayPracticeRowView), false);

    public bool HasStreak
    {
        get => (bool)GetValue(HasStreakProperty);
        set => SetValue(HasStreakProperty, value);
    }

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly BindableProperty ReasonTextProperty =
        BindableProperty.Create(nameof(ReasonText), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public string ReasonText
    {
        get => (string)GetValue(ReasonTextProperty);
        set => SetValue(ReasonTextProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(TodayPracticeRowView), null);

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }
}
