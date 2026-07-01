using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Presentation.Shared.Common;

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
            propertyChanged: OnChartPointsChanged);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TestScoreTrendChartView), string.Empty);

    public IReadOnlyList<TestScoreChartPoint> ChartPoints
    {
        get => (IReadOnlyList<TestScoreChartPoint>)GetValue(ChartPointsProperty);
        set => SetValue(ChartPointsProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    private static void OnChartPointsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TestScoreTrendChartView view && newValue is IReadOnlyList<TestScoreChartPoint> points)
        {
            view._drawable.Points = points;
            view.ChartView.Invalidate();
        }
    }
}
