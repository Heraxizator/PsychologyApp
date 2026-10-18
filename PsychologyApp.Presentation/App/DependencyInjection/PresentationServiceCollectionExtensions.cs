using PsychologyApp.Presentation.Features.Onboarding.DependencyInjection;
using PsychologyApp.Presentation.Features.ReadArticles.DependencyInjection;
using PsychologyApp.Presentation.Features.PlayMusic.DependencyInjection;
using PsychologyApp.Presentation.Features.ManageQuotes.DependencyInjection;
using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;
using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;
using PsychologyApp.Presentation.Features.ManageJournal.DependencyInjection;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Features.SendReviewForm.DependencyInjection;
using PsychologyApp.Presentation.Features.ClinicalCare.DependencyInjection;
using PsychologyApp.Presentation.Features.Chat.DependencyInjection;

namespace PsychologyApp.Presentation.App.DependencyInjection;

public static class PresentationServiceCollectionExtensions
{
    public static IServiceCollection AddPsychologyAppPresentation(this IServiceCollection services)
    {
        services.AddSharedPresentation();
        services.AddSendReviewFormFeature();
        services.AddOnboardingFeature();
        services.AddClinicalCareFeature();
        services.AddPlayMusicFeature();
        services.AddManageQuotesFeature();
        services.AddRunTestsFeature();
        services.AddManageProfileFeature();
        services.AddManageJournalFeature();
        services.AddRunTechniqueSessionFeature();
        services.AddChatFeature();
        services.AddReadArticlesFeature();
        return services;
    }
}
