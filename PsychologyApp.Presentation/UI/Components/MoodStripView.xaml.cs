using PsychologyApp.Presentation.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class MoodStripView : ContentView
{
    public MoodStripView()
    {
        InitializeComponent();
        VisualElementPressFeedback.AttachToTemplateRoot(this, new PressFeedbackOptions { HapticOnRelease = true });
    }

    public static readonly BindableProperty QuestionTextProperty =
        BindableProperty.Create(nameof(QuestionText), typeof(string), typeof(MoodStripView), string.Empty);

    public string QuestionText
    {
        get => (string)GetValue(QuestionTextProperty);
        set => SetValue(QuestionTextProperty, value);
    }

    public static readonly BindableProperty RecordMoodCommandProperty =
        BindableProperty.Create(nameof(RecordMoodCommand), typeof(ICommand), typeof(MoodStripView), null);

    public ICommand? RecordMoodCommand
    {
        get => (ICommand?)GetValue(RecordMoodCommandProperty);
        set => SetValue(RecordMoodCommandProperty, value);
    }

    public static readonly BindableProperty SelectedMoodLevelProperty =
        BindableProperty.Create(nameof(SelectedMoodLevel), typeof(int), typeof(MoodStripView), 0);

    public int SelectedMoodLevel
    {
        get => (int)GetValue(SelectedMoodLevelProperty);
        set => SetValue(SelectedMoodLevelProperty, value);
    }

    public static readonly BindableProperty TodayMoodDisplayProperty =
        BindableProperty.Create(nameof(TodayMoodDisplay), typeof(string), typeof(MoodStripView), string.Empty);

    public string TodayMoodDisplay
    {
        get => (string)GetValue(TodayMoodDisplayProperty);
        set => SetValue(TodayMoodDisplayProperty, value);
    }

    public static readonly BindableProperty HasTodayMoodProperty =
        BindableProperty.Create(nameof(HasTodayMood), typeof(bool), typeof(MoodStripView), false);

    public bool HasTodayMood
    {
        get => (bool)GetValue(HasTodayMoodProperty);
        set => SetValue(HasTodayMoodProperty, value);
    }

    public static readonly BindableProperty MoodHistorySummaryProperty =
        BindableProperty.Create(nameof(MoodHistorySummary), typeof(string), typeof(MoodStripView), string.Empty);

    public string MoodHistorySummary
    {
        get => (string)GetValue(MoodHistorySummaryProperty);
        set => SetValue(MoodHistorySummaryProperty, value);
    }

    public static readonly BindableProperty HasMoodHistorySummaryProperty =
        BindableProperty.Create(nameof(HasMoodHistorySummary), typeof(bool), typeof(MoodStripView), false);

    public bool HasMoodHistorySummary
    {
        get => (bool)GetValue(HasMoodHistorySummaryProperty);
        set => SetValue(HasMoodHistorySummaryProperty, value);
    }
}
