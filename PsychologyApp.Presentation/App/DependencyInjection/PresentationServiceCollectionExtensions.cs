using PsychologyApp.Presentation.App.Providers;

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
