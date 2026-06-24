using Microsoft.Maui.Controls.Shapes;

namespace PsychologyApp.Presentation.Widgets.Onboarding;

public partial class OnboardingStepIndicatorView : ContentView
{
    public OnboardingStepIndicatorView()
    {
        InitializeComponent();
        BuildDots();
        if (Microsoft.Maui.Controls.Application.Current is not null)
        {
            Microsoft.Maui.Controls.Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
        }
    }

    public static readonly BindableProperty StepProperty =
        BindableProperty.Create(nameof(Step), typeof(int), typeof(OnboardingStepIndicatorView), 0, propertyChanged: OnStepOrCountChanged);

    public static readonly BindableProperty StepCountProperty =
        BindableProperty.Create(nameof(StepCount), typeof(int), typeof(OnboardingStepIndicatorView), 4, propertyChanged: OnStepOrCountChanged);

    public static readonly BindableProperty StepLabelProperty =
        BindableProperty.Create(nameof(StepLabel), typeof(string), typeof(OnboardingStepIndicatorView), string.Empty);

    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public int StepCount
    {
        get => (int)GetValue(StepCountProperty);
        set => SetValue(StepCountProperty, value);
    }

    public string StepLabel
    {
        get => (string)GetValue(StepLabelProperty);
        set => SetValue(StepLabelProperty, value);
    }

    private static void OnStepOrCountChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is OnboardingStepIndicatorView view)
        {
            view.BuildDots();
        }
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e) => BuildDots();

    private void BuildDots()
    {
        DotsLayout.Children.Clear();

        int count = Math.Max(1, StepCount);
        Color activeColor = ResolveColor("Primary");
        Color inactiveColor = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark
            ? ResolveColor("Gray600")
            : ResolveColor("Gray400");

        for (int i = 0; i < count; i++)
        {
            var dot = new Border
            {
                WidthRequest = i == Step ? 24 : 8,
                HeightRequest = 8,
                StrokeThickness = 0,
                BackgroundColor = i <= Step ? activeColor : inactiveColor
            };
            dot.StrokeShape = new RoundRectangle { CornerRadius = 4 };
            DotsLayout.Children.Add(dot);
        }
    }

    private static Color ResolveColor(string key)
    {
        if (Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out object? value) == true && value is Color color)
        {
            return color;
        }

        return Colors.Gray;
    }
}
