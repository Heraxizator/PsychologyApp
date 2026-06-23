using PsychologyApp.Presentation.Pages.TechniqueTheory;
using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.App.Routes;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Lib.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.App.Providers;

public static class RunTechniqueSessionFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddRunTechniqueSessionFeature(this IServiceCollection services)
    {
        services.AddSingleton<ITechniquePageFactory, TechniquePageFactory>();
        services.AddSingleton<ITechniqueMessenger, TechniqueMessengerService>();
        services.AddSingleton<TechniqueListBuilder>();
        services.AddSingleton<DesignerTechniqueOperations>();
        services.AddSingleton<TechniqueSessionCompletionService>();
        SharedPresentationServiceCollectionExtensions.AddTransientFactory<EntryDraftCoordinator>(services);
        SharedPresentationServiceCollectionExtensions.AddTransientFactory<PaperListDraftCoordinator>(services);
        SharedPresentationServiceCollectionExtensions.AddTransientFactory<PolarityListDraftCoordinator>(services);
        services.AddSingleton<CustomTechniqueSessionOperations>();
        services.AddSingleton<PracticeDashboardLoader>();
        services.AddSingleton<TechniquesListInitializer>();
        services.AddSingleton<ITheoryViewModelFactory, TheoryViewModelFactory>();
        services.AddSingleton<IPracticeTheoryNavigator, PracticeTheoryNavigator>();
        services.AddSingleton<INavigateToTheory, TheoryNavigationAdapter>();
        services.AddSingleton<ITechniquesViewModelFactory, TechniquesViewModelFactory>();
        services.AddSingleton<ICreatedViewModelFactory, CreatedViewModelFactory>();
        services.AddSingleton<IDesignerViewModelFactory, DesignerViewModelFactory>();
        services.AddSingleton<ITechniqueViewModelFactory, TechniqueViewModelFactory>();
        return services;
    }
}
