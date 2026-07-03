using PsychologyApp.Application.Models.Tests;

namespace PsychologyApp.Presentation.Widgets.Test;

public partial class TestHistoryEntryView : ContentView
{
    public TestHistoryEntryView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty DateTextProperty =
        BindableProperty.Create(nameof(DateText), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty SummaryTextProperty =
        BindableProperty.Create(nameof(SummaryText), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty ScoreTextProperty =
        BindableProperty.Create(nameof(ScoreText), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty TrendTextProperty =
        BindableProperty.Create(nameof(TrendText), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty DurationTextProperty =
        BindableProperty.Create(nameof(DurationText), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty HasScoreProperty =
        BindableProperty.Create(nameof(HasScore), typeof(bool), typeof(TestHistoryEntryView), false);

    public static readonly BindableProperty HasTrendProperty =
        BindableProperty.Create(nameof(HasTrend), typeof(bool), typeof(TestHistoryEntryView), false);

    public static readonly BindableProperty HasDetailProperty =
        BindableProperty.Create(nameof(HasDetail), typeof(bool), typeof(TestHistoryEntryView), false);

    public static readonly BindableProperty IsWorseProperty =
        BindableProperty.Create(nameof(IsWorse), typeof(bool), typeof(TestHistoryEntryView), false);

    public static readonly BindableProperty DetailQuestionsProperty =
        BindableProperty.Create(
            nameof(DetailQuestions),
            typeof(IReadOnlyList<QuestionnaireResultQuestion>),
            typeof(TestHistoryEntryView),
            Array.Empty<QuestionnaireResultQuestion>());

    public string DateText
    {
        get => (string)GetValue(DateTextProperty);
        set => SetValue(DateTextProperty, value);
    }

    public string SummaryText
    {
        get => (string)GetValue(SummaryTextProperty);
        set => SetValue(SummaryTextProperty, value);
    }

    public string ScoreText
    {
        get => (string)GetValue(ScoreTextProperty);
        set => SetValue(ScoreTextProperty, value);
    }

    public string TrendText
    {
        get => (string)GetValue(TrendTextProperty);
        set => SetValue(TrendTextProperty, value);
    }

    public string DurationText
    {
        get => (string)GetValue(DurationTextProperty);
        set => SetValue(DurationTextProperty, value);
    }

    public bool HasScore
    {
        get => (bool)GetValue(HasScoreProperty);
        set => SetValue(HasScoreProperty, value);
    }

    public bool HasTrend
    {
        get => (bool)GetValue(HasTrendProperty);
        set => SetValue(HasTrendProperty, value);
    }

    public bool HasDetail
    {
        get => (bool)GetValue(HasDetailProperty);
        set => SetValue(HasDetailProperty, value);
    }

    public bool IsWorse
    {
        get => (bool)GetValue(IsWorseProperty);
        set => SetValue(IsWorseProperty, value);
    }

    public IReadOnlyList<QuestionnaireResultQuestion> DetailQuestions
    {
        get => (IReadOnlyList<QuestionnaireResultQuestion>)GetValue(DetailQuestionsProperty);
        set => SetValue(DetailQuestionsProperty, value);
    }
}
