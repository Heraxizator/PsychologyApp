using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.Onboarding;

public partial class OnboardingWelcomeHeroView : ContentView
{
    public OnboardingWelcomeHeroView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty AppNameProperty =
        BindableProperty.Create(nameof(AppName), typeof(string), typeof(OnboardingWelcomeHeroView), string.Empty);

    public static readonly BindableProperty AppTaglineProperty =
        BindableProperty.Create(nameof(AppTagline), typeof(string), typeof(OnboardingWelcomeHeroView), string.Empty);

    public static readonly BindableProperty WelcomeTitleProperty =
        BindableProperty.Create(nameof(WelcomeTitle), typeof(string), typeof(OnboardingWelcomeHeroView), string.Empty);

    public static readonly BindableProperty WelcomeBodyProperty =
        BindableProperty.Create(nameof(WelcomeBody), typeof(string), typeof(OnboardingWelcomeHeroView), string.Empty);

    public string AppName
    {
        get => (string)GetValue(AppNameProperty);
        set => SetValue(AppNameProperty, value);
    }

    public string AppTagline
    {
        get => (string)GetValue(AppTaglineProperty);
        set => SetValue(AppTaglineProperty, value);
    }

    public string WelcomeTitle
    {
        get => (string)GetValue(WelcomeTitleProperty);
        set => SetValue(WelcomeTitleProperty, value);
    }

    public string WelcomeBody
    {
        get => (string)GetValue(WelcomeBodyProperty);
        set => SetValue(WelcomeBodyProperty, value);
    }

    public Task PulseLogoAsync() =>
        UiAnimations.SafePulseAsync(LogoImage);
}
