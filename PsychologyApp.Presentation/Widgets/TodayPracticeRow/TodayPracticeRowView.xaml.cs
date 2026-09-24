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

    public static readonly BindableProperty NudgeTextProperty =
        BindableProperty.Create(nameof(NudgeText), typeof(string), typeof(TodayPracticeRowView), string.Empty, propertyChanged: OnHintRelevantChanged);

    public string NudgeText
    {
        get => (string)GetValue(NudgeTextProperty);
        set => SetValue(NudgeTextProperty, value);
    }

    public static readonly BindableProperty HasNudgeProperty =
        BindableProperty.Create(nameof(HasNudge), typeof(bool), typeof(TodayPracticeRowView), false, propertyChanged: OnHintRelevantChanged);

    public bool HasNudge
    {
        get => (bool)GetValue(HasNudgeProperty);
        set => SetValue(HasNudgeProperty, value);
    }

    public static readonly BindableProperty ReasonTextProperty =
        BindableProperty.Create(nameof(ReasonText), typeof(string), typeof(TodayPracticeRowView), string.Empty, propertyChanged: OnHintRelevantChanged);

    public string ReasonText
    {
        get => (string)GetValue(ReasonTextProperty);
        set => SetValue(ReasonTextProperty, value);
    }

    public static readonly BindableProperty MetaTextProperty =
        BindableProperty.Create(nameof(MetaText), typeof(string), typeof(TodayPracticeRowView), string.Empty, propertyChanged: OnHintRelevantChanged);

    public string MetaText
    {
        get => (string)GetValue(MetaTextProperty);
        set => SetValue(MetaTextProperty, value);
    }

    public static readonly BindableProperty HasMetaProperty =
        BindableProperty.Create(nameof(HasMeta), typeof(bool), typeof(TodayPracticeRowView), false, propertyChanged: OnHintRelevantChanged);

    public bool HasMeta
    {
        get => (bool)GetValue(HasMetaProperty);
        set => SetValue(HasMetaProperty, value);
    }

    private static readonly BindablePropertyKey HintTextPropertyKey =
        BindableProperty.CreateReadOnly(nameof(HintText), typeof(string), typeof(TodayPracticeRowView), string.Empty);

    public static readonly BindableProperty HintTextProperty = HintTextPropertyKey.BindableProperty;

    /// <summary>The one explanatory line under the title: a nudge, a recommendation reason or the program's status —
    /// never more than one at a time, so this single slot stands in for what used to be three separate rows.</summary>
    public string HintText
    {
        get => (string)GetValue(HintTextProperty);
        private set => SetValue(HintTextPropertyKey, value);
    }

    private static readonly BindablePropertyKey HasHintPropertyKey =
        BindableProperty.CreateReadOnly(nameof(HasHint), typeof(bool), typeof(TodayPracticeRowView), false);

    public static readonly BindableProperty HasHintProperty = HasHintPropertyKey.BindableProperty;

    public bool HasHint
    {
        get => (bool)GetValue(HasHintProperty);
        private set => SetValue(HasHintPropertyKey, value);
    }

    private static void OnHintRelevantChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not TodayPracticeRowView view)
        {
            return;
        }

        string hint = view.HasNudge && !string.IsNullOrWhiteSpace(view.NudgeText)
            ? view.NudgeText
            : !string.IsNullOrWhiteSpace(view.ReasonText)
                ? view.ReasonText
                : view.HasMeta && !string.IsNullOrWhiteSpace(view.MetaText)
                    ? view.MetaText
                    : string.Empty;

        view.HintText = hint;
        view.HasHint = hint.Length > 0;
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(TodayPracticeRowView), null);

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }
}
