using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels.Practice.Constructor;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.ViewModels.Practice.Techniques;

namespace PsychologyApp.Presentation.Services.Factories;

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
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ICreatedViewModelFactory
{
    public CreatedViewModel Create(INavigation navigation, long techniqueId) =>
        new(navigation, techniqueId, dialogService, techniqueService, techniqueMessenger, logger, settings, navigationServiceFactory(NavigationContext.From(navigation)), userProgressService);
}

public interface IDesignerViewModelFactory
{
    DesignerViewModel Create(INavigation navigation, long techniqueId);
}

public sealed class DesignerViewModelFactory(
    ITechniqueService techniqueService,
    ITechniqueMessenger techniqueMessenger,
    ILogger<DesignerViewModel> logger,
    IOptions<AppSettings> settings,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : IDesignerViewModelFactory
{
    public DesignerViewModel Create(INavigation navigation, long techniqueId) =>
        new(navigation, techniqueId, techniqueService, techniqueMessenger, logger, settings, navigationServiceFactory(NavigationContext.From(navigation)));
}

public interface ITechniqueViewModelFactory
{
    BaseViewModel Create(TechniqueId techniqueId, INavigation navigation);
}

public sealed class TechniqueViewModelFactory(
    Func<NavigationContext, INavigationService> navigationServiceFactory,
    IUserProgressService userProgressService,
    IDialogService dialogService) : ITechniqueViewModelFactory
{
    public BaseViewModel Create(TechniqueId techniqueId, INavigation navigation)
    {
        INavigationService navigationService = navigationServiceFactory(NavigationContext.From(navigation));
        return techniqueId switch
        {
            TechniqueId.Paper => new PaperListViewModel(navigationService, TechniqueId.Paper, clearTextAfterAdd: true, userProgressService, dialogService),
            TechniqueId.Polarity => new PolarityViewModel(navigationService, userProgressService, dialogService),
            TechniqueId.Copied => new PaperListViewModel(navigationService, TechniqueId.Copied, clearTextAfterAdd: false, userProgressService, dialogService),
            _ => new TechniqueSessionViewModel(navigation, techniqueId, navigationService, userProgressService, dialogService)
        };
    }
}
