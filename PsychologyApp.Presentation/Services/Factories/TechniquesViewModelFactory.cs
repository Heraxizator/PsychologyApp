using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Modules.Practice.Messages;
using PsychologyApp.Presentation.Technique.Main;

namespace PsychologyApp.Presentation.Services.Factories;

public interface ITechniquesViewModelFactory
{
    TechniquesViewModel Create(INavigation navigation);
}

public sealed class TechniquesViewModelFactory(
    ITechniqueService techniqueService,
    IToastService toastService,
    ITechniqueMessenger techniqueMessenger,
    Func<INavigation, INavigationService> navigationServiceFactory,
    IOptions<AppSettings> settings) : ITechniquesViewModelFactory
{
    public TechniquesViewModel Create(INavigation navigation) =>
        new(navigation, techniqueService, toastService, techniqueMessenger, navigationServiceFactory, settings);
}
