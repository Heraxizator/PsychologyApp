using PsychologyApp.Presentation.Features.RunTechniqueSession.Polarity;
using PsychologyApp.Presentation.Features.RunTechniqueSession.PaperList;
using PsychologyApp.Presentation.Pages.TechniqueCreated;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Technique;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.TechniqueDesigner;
using PsychologyApp.Presentation.Shared.ViewModels;
using PsychologyApp.Presentation.Pages.TechniqueSession;

namespace PsychologyApp.Presentation.App.Providers;

public interface ICreatedViewModelFactory
{
    CreatedViewModel Create(INavigation navigation, long techniqueId);
}

public sealed class CreatedViewModelFactory(
    ITechniqueService techniqueService,
    IDialogService dialogService,
    ITechniqueMessenger techniqueMessenger,
    ILogger<CreatedViewModel> logger,
    IOptions<AppSettings> settings,
    IUserProgressService userProgressService,
    CustomTechniqueSessionOperations sessionOperations,
    TechniqueSessionCompletionService sessionCompletionService,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, ICreatedViewModelFactory
{
    public CreatedViewModel Create(INavigation navigation, long techniqueId) =>
        new(
            techniqueId,
            dialogService,
            techniqueService,
            techniqueMessenger,
            logger,
            settings,
            ResolveNavigation(navigationServiceFactory, navigation),
            userProgressService,
            sessionOperations,
            sessionCompletionService);
}

public interface IDesignerViewModelFactory
{
    DesignerViewModel Create(INavigation navigation, long techniqueId);
}

public sealed class DesignerViewModelFactory(
    ITechniqueService techniqueService,
    ITechniqueMessenger techniqueMessenger,
    DesignerTechniqueOperations techniqueOperations,
    ILogger<DesignerViewModel> logger,
    IOptions<AppSettings> settings,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IDesignerViewModelFactory
{
    public DesignerViewModel Create(INavigation navigation, long techniqueId) =>
        new(techniqueId, techniqueService, techniqueMessenger, techniqueOperations, logger, settings, ResolveNavigation(navigationServiceFactory, navigation));
}

public interface ITechniqueViewModelFactory
{
    BaseViewModel Create(TechniqueId techniqueId, INavigation navigation);
}

public sealed class TechniqueViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IUserProgressService userProgressService,
    IDialogService dialogService,
    TechniqueSessionCompletionService sessionCompletionService,
    Func<EntryDraftCoordinator> entryDraftCoordinatorFactory,
    Func<PaperListDraftCoordinator> paperListDraftCoordinatorFactory,
    Func<PolarityListDraftCoordinator> polarityListDraftCoordinatorFactory) : ViewModelFactoryBase, ITechniqueViewModelFactory
{
    public BaseViewModel Create(TechniqueId techniqueId, INavigation navigation)
    {
        INavigationService navigationService = ResolveNavigation(navigationServiceFactory, navigation);
        ListTechniqueSessionHelper sessionHelper = new(
            sessionCompletionService,
            userProgressService,
            navigationService,
            dialogService);

        return techniqueId switch
        {
            TechniqueId.Paper => new PaperListViewModel(navigationService, TechniqueId.Paper, clearTextAfterAdd: true, userProgressService, sessionHelper, paperListDraftCoordinatorFactory()),
            TechniqueId.Polarity => new PolarityViewModel(navigationService, userProgressService, sessionHelper, polarityListDraftCoordinatorFactory()),
            TechniqueId.Copied => new PaperListViewModel(navigationService, TechniqueId.Copied, clearTextAfterAdd: false, userProgressService, sessionHelper, paperListDraftCoordinatorFactory()),
            _ => new TechniqueSessionViewModel(techniqueId, navigationService, userProgressService, sessionHelper, entryDraftCoordinatorFactory())
        };
    }
}
