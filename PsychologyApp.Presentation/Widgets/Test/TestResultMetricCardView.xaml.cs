namespace PsychologyApp.Presentation.Widgets.Test;

public partial class TestResultMetricCardView : ContentView
{
    public TestResultMetricCardView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty MetricNameProperty =
        BindableProperty.Create(nameof(MetricName), typeof(string), typeof(TestResultMetricCardView), string.Empty);

    public static readonly BindableProperty MetricValueProperty =
        BindableProperty.Create(nameof(MetricValue), typeof(string), typeof(TestResultMetricCardView), string.Empty);

    public static readonly BindableProperty MetricDescriptionProperty =
        BindableProperty.Create(nameof(MetricDescription), typeof(string), typeof(TestResultMetricCardView), string.Empty);

    public string MetricName
    {
        get => (string)GetValue(MetricNameProperty);
        set => SetValue(MetricNameProperty, value);
    }

    public string MetricValue
    {
        get => (string)GetValue(MetricValueProperty);
        set => SetValue(MetricValueProperty, value);
    }

    public string MetricDescription
    {
        get => (string)GetValue(MetricDescriptionProperty);
        set => SetValue(MetricDescriptionProperty, value);
    }
}
