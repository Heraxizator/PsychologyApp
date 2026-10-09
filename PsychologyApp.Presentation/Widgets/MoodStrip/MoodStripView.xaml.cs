using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.MoodStrip;

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
        BindableProperty.Create(
            nameof(SelectedMoodLevel),
            typeof(int),
            typeof(MoodStripView),
            0,
            propertyChanged: OnSelectedMoodLevelChanged);

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

    public static readonly BindableProperty UsePlainLayoutProperty =
        BindableProperty.Create(nameof(UsePlainLayout), typeof(bool), typeof(MoodStripView), false);

    public bool UsePlainLayout
    {
        get => (bool)GetValue(UsePlainLayoutProperty);
        set => SetValue(UsePlainLayoutProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        ApplySelection(SelectedMoodLevel);
    }

    /// <summary>
    /// The chosen face gets the selected style and every other face the plain one. A style is swapped as a whole, so nothing is left behind on a
    /// face that is no longer chosen (a trigger that set the background of a border did not always give it back).
    /// </summary>
    private void ApplySelection(int selected)
    {
        for (int level = 1; level <= 5; level++)
        {
            Border? chip = (GetTemplateChild($"MoodChip{level}") ?? FindByName($"MoodChip{level}")) as Border;
            string key = level == selected ? "MoodChipSelectedStyle" : "MoodChipStyle";
            if (chip is not null
                && Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out object? style) == true
                && style is Style found)
            {
                chip.Style = found;
            }
        }
    }

    private static void OnSelectedMoodLevelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not MoodStripView strip)
        {
            return;
        }

        if (newValue is int changedTo)
        {
            strip.ApplySelection(changedTo);
        }

        if (newValue is int level && level >= 1 && level <= 5 && !Equals(oldValue, newValue))
        {
            strip.PulseSelectedChip(level);
        }
    }

    private void PulseSelectedChip(int level)
    {
        VisualElement? chip = GetTemplateChild($"MoodChip{level}") as VisualElement
            ?? FindByName($"MoodChip{level}") as VisualElement;
        UiAnimations.SafePulseAsync(chip ?? this).FireAndForget();
    }
}
