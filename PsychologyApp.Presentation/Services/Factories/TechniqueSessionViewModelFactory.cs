using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Modules.Practice.Messages;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.ViewModels.Practice.Constructor;
using PsychologyApp.Presentation.ViewModels;
using PsychologyApp.Presentation.ViewModels.TechniqueViewModels;

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
    Func<INavigation, INavigationService> navigationServiceFactory) : ICreatedViewModelFactory
{
    public CreatedViewModel Create(INavigation navigation, long techniqueId) =>
        new(navigation, techniqueId, dialogService, techniqueService, techniqueMessenger, logger, settings, navigationServiceFactory(navigation));
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
    Func<INavigation, INavigationService> navigationServiceFactory) : IDesignerViewModelFactory
{
    public DesignerViewModel Create(INavigation navigation, long techniqueId) =>
        new(navigation, techniqueId, techniqueService, techniqueMessenger, logger, settings, navigationServiceFactory(navigation));
}

public interface ITechniqueViewModelFactory
{
    BaseViewModel Create(TechniqueId techniqueId, INavigation navigation);
}

public sealed class TechniqueViewModelFactory(Func<INavigation, INavigationService> navigationServiceFactory) : ITechniqueViewModelFactory
{
    public BaseViewModel Create(TechniqueId techniqueId, INavigation navigation)
    {
        INavigationService navigationService = navigationServiceFactory(navigation);
        return techniqueId switch
        {
            TechniqueId.Paper => new PaperListViewModel(navigationService, TechniqueId.Paper, clearTextAfterAdd: true),
            TechniqueId.Polarity => new PolarityViewModel(navigationService),
            TechniqueId.Copied => new PaperListViewModel(navigationService, TechniqueId.Copied, clearTextAfterAdd: false),
            _ => new TechniqueSessionViewModel(navigation, techniqueId, navigationService)
        };
    }
}
