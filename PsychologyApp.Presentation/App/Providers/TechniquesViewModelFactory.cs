using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Technique;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.Techniques;

namespace PsychologyApp.Presentation.App.Providers;

public interface ITechniquesViewModelFactory
{
    TechniquesViewModel Create(INavigation navigation);
}

public sealed class TechniquesViewModelFactory(
    ITechniqueService techniqueService,
    IToastService toastService,
    ITechniqueMessenger techniqueMessenger,
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IUserProgressService userProgressService,
    TechniqueListBuilder techniqueListBuilder,
    IDatabaseReadySignal databaseReadySignal,
    PracticeDashboardLoader dashboardLoader,
    TechniquesListInitializer listInitializer,
    IOptions<AppSettings> settings,
    ILogger<TechniquesViewModel> logger) : ViewModelFactoryBase, ITechniquesViewModelFactory
{
    public TechniquesViewModel Create(INavigation navigation) =>
        new(
            techniqueService,
            toastService,
            techniqueMessenger,
            ResolveNavigation(navigationServiceFactory, navigation),
            userProgressService,
            techniqueListBuilder,
            databaseReadySignal,
            dashboardLoader,
            listInitializer,
            settings,
            logger);
}
