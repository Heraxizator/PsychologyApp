using PsychologyApp.Presentation.Shared.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.TodayPracticeRow;

public partial class TodayPracticeRowView : ContentView
{
    public TodayPracticeRowView()
    {
        InitializeComponent();
        VisualElementPressFeedback.AttachToTemplateRoot(this, new PressFeedbackOptions { HapticOnRelease = true });
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

    public static readonly BindableProperty DateTextProperty =
        BindableProperty.Create(nameof(DateText), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public string DateText
    {
        get => (string)GetValue(DateTextProperty);
        set => SetValue(DateTextProperty, value);
    }

    public static readonly BindableProperty NudgeTextProperty =
        BindableProperty.Create(nameof(NudgeText), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public string NudgeText
    {
        get => (string)GetValue(NudgeTextProperty);
        set => SetValue(NudgeTextProperty, value);
    }

    public static readonly BindableProperty HasNudgeProperty =
        BindableProperty.Create(nameof(HasNudge), typeof(bool), typeof(TodayPracticeRowView), false, propertyChanged: OnReasonRelevantChanged);

    public bool HasNudge
    {
        get => (bool)GetValue(HasNudgeProperty);
        set => SetValue(HasNudgeProperty, value);
    }

    public static readonly BindableProperty ReasonTextProperty =
        BindableProperty.Create(nameof(ReasonText), typeof(string), typeof(TodayPracticeRowView), string.Empty, propertyChanged: OnReasonRelevantChanged);

    public string ReasonText
    {
        get => (string)GetValue(ReasonTextProperty);
        set => SetValue(ReasonTextProperty, value);
    }

    private static readonly BindablePropertyKey HasReasonOnlyPropertyKey =
        BindableProperty.CreateReadOnly(nameof(HasReasonOnly), typeof(bool), typeof(TodayPracticeRowView), false);

    public static readonly BindableProperty HasReasonOnlyProperty = HasReasonOnlyPropertyKey.BindableProperty;

    /// <summary>The reason line is shown only when there is no nudge: the two never compete for the same row.</summary>
    public bool HasReasonOnly
    {
        get => (bool)GetValue(HasReasonOnlyProperty);
        private set => SetValue(HasReasonOnlyPropertyKey, value);
    }

    private static void OnReasonRelevantChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TodayPracticeRowView view)
        {
            view.HasReasonOnly = !view.HasNudge && !string.IsNullOrWhiteSpace(view.ReasonText);
        }
    }

    public static readonly BindableProperty MetaTextProperty =
        BindableProperty.Create(nameof(MetaText), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public string MetaText
    {
        get => (string)GetValue(MetaTextProperty);
        set => SetValue(MetaTextProperty, value);
    }

    public static readonly BindableProperty HasMetaProperty =
        BindableProperty.Create(nameof(HasMeta), typeof(bool), typeof(TodayPracticeRowView), false);

    public bool HasMeta
    {
        get => (bool)GetValue(HasMetaProperty);
        set => SetValue(HasMetaProperty, value);
    }

    public static readonly BindableProperty ActionTextProperty =
        BindableProperty.Create(nameof(ActionText), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public string ActionText
    {
        get => (string)GetValue(ActionTextProperty);
        set => SetValue(ActionTextProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(TodayPracticeRowView), null);

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }
}
