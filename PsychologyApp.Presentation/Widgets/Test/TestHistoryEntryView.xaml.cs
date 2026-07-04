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

    public static readonly BindableProperty HasLuscherDetailProperty =
        BindableProperty.Create(nameof(HasLuscherDetail), typeof(bool), typeof(TestHistoryEntryView), false);

    public static readonly BindableProperty HasStandardLuscherDetailProperty =
        BindableProperty.Create(nameof(HasStandardLuscherDetail), typeof(bool), typeof(TestHistoryEntryView), false);

    public static readonly BindableProperty HasBriefLuscherDetailProperty =
        BindableProperty.Create(nameof(HasBriefLuscherDetail), typeof(bool), typeof(TestHistoryEntryView), false);

    public static readonly BindableProperty LuscherFirstPassTitleProperty =
        BindableProperty.Create(nameof(LuscherFirstPassTitle), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty LuscherFirstPassTextProperty =
        BindableProperty.Create(nameof(LuscherFirstPassText), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty LuscherSecondPassTitleProperty =
        BindableProperty.Create(nameof(LuscherSecondPassTitle), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty LuscherSecondPassTextProperty =
        BindableProperty.Create(nameof(LuscherSecondPassText), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty LuscherBkTextProperty =
        BindableProperty.Create(nameof(LuscherBkText), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty LuscherBriefFirstTitleProperty =
        BindableProperty.Create(nameof(LuscherBriefFirstTitle), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty LuscherBriefFirstTextProperty =
        BindableProperty.Create(nameof(LuscherBriefFirstText), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty LuscherBriefSecondTitleProperty =
        BindableProperty.Create(nameof(LuscherBriefSecondTitle), typeof(string), typeof(TestHistoryEntryView), string.Empty);

    public static readonly BindableProperty LuscherBriefSecondTextProperty =
        BindableProperty.Create(nameof(LuscherBriefSecondText), typeof(string), typeof(TestHistoryEntryView), string.Empty);

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

    public bool HasLuscherDetail
    {
        get => (bool)GetValue(HasLuscherDetailProperty);
        set => SetValue(HasLuscherDetailProperty, value);
    }

    public bool HasStandardLuscherDetail
    {
        get => (bool)GetValue(HasStandardLuscherDetailProperty);
        set => SetValue(HasStandardLuscherDetailProperty, value);
    }

    public bool HasBriefLuscherDetail
    {
        get => (bool)GetValue(HasBriefLuscherDetailProperty);
        set => SetValue(HasBriefLuscherDetailProperty, value);
    }

    public string LuscherFirstPassTitle
    {
        get => (string)GetValue(LuscherFirstPassTitleProperty);
        set => SetValue(LuscherFirstPassTitleProperty, value);
    }

    public string LuscherFirstPassText
    {
        get => (string)GetValue(LuscherFirstPassTextProperty);
        set => SetValue(LuscherFirstPassTextProperty, value);
    }

    public string LuscherSecondPassTitle
    {
        get => (string)GetValue(LuscherSecondPassTitleProperty);
        set => SetValue(LuscherSecondPassTitleProperty, value);
    }

    public string LuscherSecondPassText
    {
        get => (string)GetValue(LuscherSecondPassTextProperty);
        set => SetValue(LuscherSecondPassTextProperty, value);
    }

    public string LuscherBkText
    {
        get => (string)GetValue(LuscherBkTextProperty);
        set => SetValue(LuscherBkTextProperty, value);
    }

    public string LuscherBriefFirstTitle
    {
        get => (string)GetValue(LuscherBriefFirstTitleProperty);
        set => SetValue(LuscherBriefFirstTitleProperty, value);
    }

    public string LuscherBriefFirstText
    {
        get => (string)GetValue(LuscherBriefFirstTextProperty);
        set => SetValue(LuscherBriefFirstTextProperty, value);
    }

    public string LuscherBriefSecondTitle
    {
        get => (string)GetValue(LuscherBriefSecondTitleProperty);
        set => SetValue(LuscherBriefSecondTitleProperty, value);
    }

    public string LuscherBriefSecondText
    {
        get => (string)GetValue(LuscherBriefSecondTextProperty);
        set => SetValue(LuscherBriefSecondTextProperty, value);
    }
}
