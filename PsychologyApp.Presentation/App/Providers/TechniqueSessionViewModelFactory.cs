using PsychologyApp.Application.Practice;
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
    CreatedViewModel Create(ContentPage page, long techniqueId);
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
    public CreatedViewModel Create(ContentPage page, long techniqueId) =>
        new(
            techniqueId,
            dialogService,
            techniqueService,
            techniqueMessenger,
            logger,
            settings,
            ResolveNavigation(navigationServiceFactory, page),
            userProgressService,
            sessionOperations,
            sessionCompletionService);
}

public interface IDesignerViewModelFactory
{
    DesignerViewModel Create(ContentPage page, long techniqueId);
}

public sealed class DesignerViewModelFactory(
    ITechniqueService techniqueService,
    ITechniqueMessenger techniqueMessenger,
    DesignerTechniqueOperations techniqueOperations,
    ILogger<DesignerViewModel> logger,
    IOptions<AppSettings> settings,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IDesignerViewModelFactory
{
    public DesignerViewModel Create(ContentPage page, long techniqueId) =>
        new(techniqueId, techniqueService, techniqueMessenger, techniqueOperations, logger, settings, ResolveNavigation(navigationServiceFactory, page));
}

public interface ITechniqueViewModelFactory
{
    BaseViewModel Create(TechniqueId techniqueId, INavigation navigation);
}

public sealed class TechniqueViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    ITechniqueCatalogService techniqueCatalogService,
    IUserProgressService userProgressService,
    IDialogService dialogService,
    TechniqueSessionCompletionService sessionCompletionService,
    Func<EntryDraftCoordinator> entryDraftCoordinatorFactory,
    Func<PaperListDraftCoordinator> paperListDraftCoordinatorFactory,
    Func<PolarityListDraftCoordinator> polarityListDraftCoordinatorFactory) : ViewModelFactoryBase, ITechniqueViewModelFactory
{
    public BaseViewModel Create(TechniqueId techniqueId, INavigation navigation)
    {
        INavigationService navigationService = navigationServiceFactory(NavigationContext.From(navigation));
        ListTechniqueSessionHelper sessionHelper = new(
            sessionCompletionService,
            userProgressService,
            navigationService,
            dialogService);

        return techniqueId switch
        {
            TechniqueId.Paper => new PaperListViewModel(navigationService, TechniqueId.Paper, clearTextAfterAdd: true, userProgressService, sessionHelper, paperListDraftCoordinatorFactory(), techniqueCatalogService),
            TechniqueId.Polarity => new PolarityViewModel(navigationService, userProgressService, sessionHelper, polarityListDraftCoordinatorFactory(), techniqueCatalogService),
            TechniqueId.Copied => new PaperListViewModel(navigationService, TechniqueId.Copied, clearTextAfterAdd: false, userProgressService, sessionHelper, paperListDraftCoordinatorFactory(), techniqueCatalogService),
            _ => new TechniqueSessionViewModel(techniqueId, navigationService, userProgressService, sessionHelper, entryDraftCoordinatorFactory(), techniqueCatalogService)
        };
    }
}
