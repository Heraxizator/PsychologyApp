namespace PsychologyApp.Presentation.Widgets.Test;

public partial class TestResultHeroView : ContentView
{
    public TestResultHeroView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty ScoreTitleProperty =
        BindableProperty.Create(nameof(ScoreTitle), typeof(string), typeof(TestResultHeroView), string.Empty);

    public static readonly BindableProperty InterpretationProperty =
        BindableProperty.Create(nameof(Interpretation), typeof(string), typeof(TestResultHeroView), string.Empty);

    public static readonly BindableProperty TrendTextProperty =
        BindableProperty.Create(nameof(TrendText), typeof(string), typeof(TestResultHeroView), string.Empty);

    public static readonly BindableProperty HasTrendBadgeProperty =
        BindableProperty.Create(nameof(HasTrendBadge), typeof(bool), typeof(TestResultHeroView), false);

    public string ScoreTitle
    {
        get => (string)GetValue(ScoreTitleProperty);
        set => SetValue(ScoreTitleProperty, value);
    }

    public string Interpretation
    {
        get => (string)GetValue(InterpretationProperty);
        set => SetValue(InterpretationProperty, value);
    }

    public string TrendText
    {
        get => (string)GetValue(TrendTextProperty);
        set => SetValue(TrendTextProperty, value);
    }

    public bool HasTrendBadge
    {
        get => (bool)GetValue(HasTrendBadgeProperty);
        set => SetValue(HasTrendBadgeProperty, value);
    }
}
