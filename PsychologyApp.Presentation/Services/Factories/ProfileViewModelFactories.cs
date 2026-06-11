using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels.Review;
using PsychologyApp.Presentation.ViewModels.Profile;

namespace PsychologyApp.Presentation.Services.Factories;

public interface IUserViewModelFactory
{
    UserViewModel Create(INavigation navigation);
}

public sealed class UserViewModelFactory(
    IQuotService quotService,
    IUserProgressService userProgressService,
    ILogger<UserViewModel> logger,
    IOptions<AppSettings> settings,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : IUserViewModelFactory
{
    public UserViewModel Create(INavigation navigation) =>
        new(navigation, quotService, userProgressService, logger, settings, navigationServiceFactory(NavigationContext.From(navigation)));
}

public interface IOptionsViewModelFactory
{
    OptionsViewModel Create(INavigation navigation);
}

public sealed class OptionsViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : IOptionsViewModelFactory
{
    public OptionsViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)));
}

public interface IDonateViewModelFactory
{
    DonateViewModel Create(INavigation navigation);
}

public sealed class DonateViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : IDonateViewModelFactory
{
    public DonateViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)));
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
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ISettingsViewModelFactory
{
    public SettingsViewModel Create(INavigation navigation) =>
        new(navigation, dialogService, navigationServiceFactory(NavigationContext.From(navigation)));
}

public interface IInfoViewModelFactory
{
    InfoViewModel Create(INavigation navigation);
}

public sealed class InfoViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : IInfoViewModelFactory
{
    public InfoViewModel Create(INavigation navigation) =>
        new(navigation, navigationServiceFactory(NavigationContext.From(navigation)));
}
