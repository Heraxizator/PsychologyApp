using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.Onboarding;

public partial class OnboardingOverviewBannerView : ContentView
{
    public OnboardingOverviewBannerView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty LeadTextProperty =
        BindableProperty.Create(nameof(LeadText), typeof(string), typeof(OnboardingOverviewBannerView), string.Empty);

    public string LeadText
    {
        get => (string)GetValue(LeadTextProperty);
        set => SetValue(LeadTextProperty, value);
    }

    public async Task PlayIconSequenceAsync(CancellationToken cancellationToken = default)
    {
        Border[] icons =
        [
            PracticeIconBorder,
            TestsIconBorder,
            SomaticIconBorder,
            MusicIconBorder,
            QuotesIconBorder
        ];

        foreach (Border icon in icons)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await UiAnimations.SafePulseAsync(icon, cancellationToken);
        }
    }
}
