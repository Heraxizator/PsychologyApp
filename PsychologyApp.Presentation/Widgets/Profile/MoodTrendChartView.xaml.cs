using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.Profile;

public partial class MoodTrendChartView : ContentView
{
    private readonly MoodTrendDrawable _drawable = new();

    public MoodTrendChartView()
    {
        InitializeComponent();
        Title = AppStrings.ProfileMoodTrendTitle;
        ChartView.Drawable = _drawable;
    }

    public static readonly BindableProperty ChartPointsProperty =
        BindableProperty.Create(
            nameof(ChartPoints),
            typeof(IReadOnlyList<MoodChartPoint>),
            typeof(MoodTrendChartView),
            Array.Empty<MoodChartPoint>(),
            propertyChanged: OnChartPointsChanged);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(
            nameof(Subtitle),
            typeof(string),
            typeof(MoodTrendChartView),
            string.Empty,
            propertyChanged: OnSubtitleChanged);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(MoodTrendChartView), string.Empty);

    public static readonly BindableProperty HasSubtitleProperty =
        BindableProperty.Create(nameof(HasSubtitle), typeof(bool), typeof(MoodTrendChartView), false);

    public IReadOnlyList<MoodChartPoint> ChartPoints
    {
        get => (IReadOnlyList<MoodChartPoint>)GetValue(ChartPointsProperty);
        set => SetValue(ChartPointsProperty, value);
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

    private static void OnChartPointsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MoodTrendChartView view && newValue is IReadOnlyList<MoodChartPoint> points)
        {
            view._drawable.Points = points;
            view.ChartView.Invalidate();
        }
    }

    private static void OnSubtitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MoodTrendChartView view)
        {
            view.HasSubtitle = newValue is string subtitle && !string.IsNullOrWhiteSpace(subtitle);
        }
    }
}
