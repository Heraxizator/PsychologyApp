using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.Onboarding;

public partial class OnboardingFinishHeaderView : ContentView
{
    public OnboardingFinishHeaderView()
    {
        InitializeComponent();
    }

    public VisualElement CheckCircleElement => CheckCircleBorder;

    public Task PulseCheckCircleAsync() =>
        UiAnimations.SafePulseAsync(CheckCircleBorder);
}
