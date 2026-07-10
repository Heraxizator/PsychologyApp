using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Widgets.Test;

public partial class TestScoreTrendChartView : ContentView
{
    private readonly TestScoreTrendDrawable _drawable = new();

    public TestScoreTrendChartView()
    {
        InitializeComponent();
        Title = AppStrings.TestHistoryTrendTitle;
        ChartView.Drawable = _drawable;
    }

    public static readonly BindableProperty ChartPointsProperty =
        BindableProperty.Create(
            nameof(ChartPoints),
            typeof(IReadOnlyList<TestScoreChartPoint>),
            typeof(TestScoreTrendChartView),
            Array.Empty<TestScoreChartPoint>(),
            propertyChanged: OnChartDataChanged);

    public static readonly BindableProperty DomainMinProperty =
        BindableProperty.Create(
            nameof(DomainMin),
            typeof(int),
            typeof(TestScoreTrendChartView),
            0,
            propertyChanged: OnChartDataChanged);

    public static readonly BindableProperty DomainMaxProperty =
        BindableProperty.Create(
            nameof(DomainMax),
            typeof(int),
            typeof(TestScoreTrendChartView),
            10,
            propertyChanged: OnChartDataChanged);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(
            nameof(Subtitle),
            typeof(string),
            typeof(TestScoreTrendChartView),
            string.Empty,
            propertyChanged: OnSubtitleChanged);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TestScoreTrendChartView), string.Empty);

    public static readonly BindableProperty HasSubtitleProperty =
        BindableProperty.Create(nameof(HasSubtitle), typeof(bool), typeof(TestScoreTrendChartView), false);

    public IReadOnlyList<TestScoreChartPoint> ChartPoints
    {
        get => (IReadOnlyList<TestScoreChartPoint>)GetValue(ChartPointsProperty);
        set => SetValue(ChartPointsProperty, value);
    }

    public int DomainMin
    {
        get => (int)GetValue(DomainMinProperty);
        set => SetValue(DomainMinProperty, value);
    }

    public int DomainMax
    {
        get => (int)GetValue(DomainMaxProperty);
        set => SetValue(DomainMaxProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool HasSubtitle
    {
        get => (bool)GetValue(HasSubtitleProperty);
        private set => SetValue(HasSubtitleProperty, value);
    }

    private static void OnChartDataChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not TestScoreTrendChartView view)
        {
            return;
        }

        view._drawable.Points = view.ChartPoints;
        view._drawable.DomainMin = view.DomainMin;
        view._drawable.DomainMax = view.DomainMax;
        view.ChartView.Invalidate();

        if (view.ChartPoints.Count >= 2)
        {
            UiAnimations.SafeFadeInAsync(view, allowHidden: true).FireAndForget();
        }
    }

    private static void OnSubtitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TestScoreTrendChartView view)
        {
            view.HasSubtitle = newValue is string subtitle && !string.IsNullOrWhiteSpace(subtitle);
        }
    }
}
