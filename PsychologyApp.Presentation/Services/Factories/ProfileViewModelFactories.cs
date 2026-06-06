using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.Statistic;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Modules.Profile;
using PsychologyApp.Presentation.ViewModels.Review;
using PsychologyApp.Presentation.ViewModels.About;
using PsychologyApp.Presentation.ViewModels.Profile;
using PsychologyApp.Presentation.ViewModels.Settings;

namespace PsychologyApp.Presentation.Services.Factories;

public interface IUserViewModelFactory
{
    UserViewModel Create(INavigation navigation);
}

public sealed class UserViewModelFactory(
    IQuotService quotService,
    IStatisticService statisticService,
    ILogger<UserViewModel> logger,
    IOptions<AppSettings> settings,
    Func<INavigation, INavigationService> navigationServiceFactory) : IUserViewModelFactory
{
    public UserViewModel Create(INavigation navigation) =>
        new(navigation, quotService, statisticService, logger, settings, navigationServiceFactory(navigation));
}

public interface IOptionsViewModelFactory
{
    OptionsViewModel Create(INavigation navigation);
}

public sealed class OptionsViewModelFactory(Func<INavigation, INavigationService> navigationServiceFactory) : IOptionsViewModelFactory
{
    public OptionsViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(navigation));
}

public interface IDonateViewModelFactory
{
    DonateViewModel Create(INavigation navigation);
}

public sealed class DonateViewModelFactory(Func<INavigation, INavigationService> navigationServiceFactory) : IDonateViewModelFactory
{
    public DonateViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(navigation));
}

public interface IFormViewModelFactory
{
    FormViewModel Create();
}

public sealed class FormViewModelFactory(IDialogService dialogService, IOptions<AppSettings> settings) : IFormViewModelFactory
{
    public FormViewModel Create() => new(dialogService, settings);
}

public interface ISettingsViewModelFactory
{
    SettingsViewModel Create(INavigation navigation);
}

public sealed class SettingsViewModelFactory(
    IDialogService dialogService,
    Func<INavigation, INavigationService> navigationServiceFactory) : ISettingsViewModelFactory
{
    public SettingsViewModel Create(INavigation navigation) =>
        new(navigation, dialogService, navigationServiceFactory(navigation));
}

public interface IInfoViewModelFactory
{
    InfoViewModel Create(INavigation navigation);
}

public sealed class InfoViewModelFactory(Func<INavigation, INavigationService> navigationServiceFactory) : IInfoViewModelFactory
{
    public InfoViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(navigation));
}
