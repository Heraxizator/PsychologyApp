using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels.Practice;

namespace PsychologyApp.Presentation.Services.Factories;

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
    IOptions<AppSettings> settings) : ViewModelFactoryBase, ITechniquesViewModelFactory
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
            settings);
}
