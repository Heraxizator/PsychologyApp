using PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;
using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.App.Routes;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Lib.Navigation;
using PsychologyApp.Presentation.Shared.Lib.Recommendations;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

public static class RunTechniqueSessionFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddRunTechniqueSessionFeature(this IServiceCollection services)
    {
        services.AddSingleton<ITechniquePageFactory, TechniquePageFactory>();
        services.AddSingleton<ITechniqueMessenger, TechniqueMessengerService>();
        services.AddSingleton<TechniqueCatalogGateway>();
        services.AddSingleton<TodayRecommendationResolver>();
        services.AddSingleton<ITodayRecommendationReasonFormatter, TodayRecommendationReasonFormatterAdapter>();
        services.AddSingleton<NextPracticeResolver>();
        services.AddSingleton<TechniqueListBuilder>();
        services.AddSingleton<DesignerTechniqueOperations>();
        services.AddSingleton<TechniqueSessionCompletionService>();
        SharedPresentationServiceCollectionExtensions.AddTransientFactory<EntryDraftCoordinator>(services);
        SharedPresentationServiceCollectionExtensions.AddTransientFactory<PaperListDraftCoordinator>(services);
        SharedPresentationServiceCollectionExtensions.AddTransientFactory<PolarityListDraftCoordinator>(services);
        services.AddSingleton<CustomTechniqueSessionOperations>();
        services.AddSingleton<PracticeDashboardLoader>();
        services.AddSingleton<PracticeClinicalDashboardEnricher>();
        services.AddSingleton<TechniquesListInitializer>();
        services.AddSingleton<ITheoryViewModelFactory, TheoryViewModelFactory>();
        services.AddSingleton<IPracticeTheoryNavigator, PracticeTheoryNavigator>();
        services.AddSingleton<INavigateToTheory, TheoryNavigationAdapter>();
        services.AddSingleton<ITechniquesViewModelFactory, TechniquesViewModelFactory>();
        services.AddSingleton<ICreatedViewModelFactory, CreatedViewModelFactory>();
        services.AddSingleton<IDesignerViewModelFactory, DesignerViewModelFactory>();
        services.AddSingleton<ITechniqueViewModelFactory, TechniqueViewModelFactory>();
        services.AddSingleton<ITechniqueDialogueViewModelFactory, TechniqueDialogueViewModelFactory>();
        services.AddSingleton<ICompanionViewModelFactory, CompanionViewModelFactory>();
        services.AddSingleton<IPracticeCompletionViewModelFactory, PracticeCompletionViewModelFactory>();
        return services;
    }
}
