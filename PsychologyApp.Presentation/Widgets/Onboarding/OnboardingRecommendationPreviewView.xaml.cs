namespace PsychologyApp.Presentation.Widgets.Onboarding;

public partial class OnboardingRecommendationPreviewView : ContentView
{
    public OnboardingRecommendationPreviewView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty CaptionTextProperty =
        BindableProperty.Create(nameof(CaptionText), typeof(string), typeof(OnboardingRecommendationPreviewView), string.Empty);

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(OnboardingRecommendationPreviewView), string.Empty);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(OnboardingRecommendationPreviewView), string.Empty);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(OnboardingRecommendationPreviewView), string.Empty);

    public static readonly BindableProperty ReasonTextProperty =
        BindableProperty.Create(nameof(ReasonText), typeof(string), typeof(OnboardingRecommendationPreviewView), string.Empty);

    public string CaptionText
    {
        get => (string)GetValue(CaptionTextProperty);
        set => SetValue(CaptionTextProperty, value);
    }

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string ReasonText
    {
        get => (string)GetValue(ReasonTextProperty);
        set => SetValue(ReasonTextProperty, value);
    }
}
