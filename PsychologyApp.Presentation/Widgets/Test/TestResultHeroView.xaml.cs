namespace PsychologyApp.Presentation.Widgets.Test;

using PsychologyApp.Domain.Tests;

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

    public static readonly BindableProperty TrendKindProperty =
        BindableProperty.Create(
            nameof(TrendKind),
            typeof(TestTrendKind),
            typeof(TestResultHeroView),
            TestTrendKind.None,
            propertyChanged: OnTrendKindChanged);

    public static readonly BindableProperty IsTrendWorseProperty =
        BindableProperty.Create(nameof(IsTrendWorse), typeof(bool), typeof(TestResultHeroView), false);

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

    public TestTrendKind TrendKind
    {
        get => (TestTrendKind)GetValue(TrendKindProperty);
        set => SetValue(TrendKindProperty, value);
    }

    public bool IsTrendWorse
    {
        get => (bool)GetValue(IsTrendWorseProperty);
        private set => SetValue(IsTrendWorseProperty, value);
    }

    private static void OnTrendKindChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TestResultHeroView view && newValue is TestTrendKind kind)
        {
            view.IsTrendWorse = kind is TestTrendKind.Worse;
        }
    }
}
