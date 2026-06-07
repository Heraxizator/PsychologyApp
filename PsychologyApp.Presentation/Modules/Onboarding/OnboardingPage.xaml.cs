using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Modules.Onboarding;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage(IOnboardingViewModelFactory factory, Func<PsychologyApp.Presentation.Modules.Practice.Techniques.TechniqueId?, Task> onCompleted)
    {
        InitializeComponent();
        BindingContext = factory.Create(Navigation, onCompleted);
    }
}
