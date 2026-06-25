namespace PsychologyApp.Presentation.Widgets.Onboarding;

public partial class OnboardingValueStripView : ContentView
{
    public OnboardingValueStripView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty OfflineTextProperty =
        BindableProperty.Create(nameof(OfflineText), typeof(string), typeof(OnboardingValueStripView), string.Empty);

    public static readonly BindableProperty NoJudgmentTextProperty =
        BindableProperty.Create(nameof(NoJudgmentText), typeof(string), typeof(OnboardingValueStripView), string.Empty);

    public static readonly BindableProperty OnDeviceTextProperty =
        BindableProperty.Create(nameof(OnDeviceText), typeof(string), typeof(OnboardingValueStripView), string.Empty);

    public string OfflineText
    {
        get => (string)GetValue(OfflineTextProperty);
        set => SetValue(OfflineTextProperty, value);
    }

    public string NoJudgmentText
    {
        get => (string)GetValue(NoJudgmentTextProperty);
        set => SetValue(NoJudgmentTextProperty, value);
    }

    public string OnDeviceText
    {
        get => (string)GetValue(OnDeviceTextProperty);
        set => SetValue(OnDeviceTextProperty, value);
    }
}
