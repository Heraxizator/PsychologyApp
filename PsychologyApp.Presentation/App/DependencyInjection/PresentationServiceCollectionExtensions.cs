using PsychologyApp.Presentation.Features.Onboarding.DependencyInjection;
using PsychologyApp.Presentation.Features.PlayMusic.DependencyInjection;
using PsychologyApp.Presentation.Features.SearchPhysics.DependencyInjection;
using PsychologyApp.Presentation.Features.ManageQuotes.DependencyInjection;
using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;
using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Features.SendReviewForm.DependencyInjection;

namespace PsychologyApp.Presentation.App.DependencyInjection;

public static class PresentationServiceCollectionExtensions
{
    public static IServiceCollection AddPsychologyAppPresentation(this IServiceCollection services)
    {
        services.AddSharedPresentation();
        services.AddSendReviewFormFeature();
        services.AddOnboardingFeature();
        services.AddPlayMusicFeature();
        services.AddSearchPhysicsFeature();
        services.AddManageQuotesFeature();
        services.AddRunTestsFeature();
        services.AddManageProfileFeature();
        services.AddRunTechniqueSessionFeature();
        return services;
    }
}
