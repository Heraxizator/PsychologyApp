using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Preferences;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.Services.Profile;
using PsychologyApp.Presentation.ViewModels.Review;
using PsychologyApp.Presentation.ViewModels.Profile;

namespace PsychologyApp.Presentation.Services.Factories;

public interface IUserViewModelFactory
{
    UserViewModel Create(INavigation navigation);
}

public sealed class UserViewModelFactory(
    ILogger<UserViewModel> logger,
    IOptions<AppSettings> settings,
    IQuotesChangeNotifier quotesChangeNotifier,
    ProfileStatsLoader profileStatsLoader,
    Func<ProfileQuotesLoader> profileQuotesLoaderFactory,
    ProfilePracticeHistoryLoader practiceHistoryLoader,
    ProfileFeaturedTechniquesBuilder featuredTechniquesBuilder,
    QuoteItemCommandsFactory quoteCommandsFactory,
    UserProfileRefreshCoordinator profileRefreshCoordinator,
    LanguageContentReloader languageContentReloader,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IUserViewModelFactory
{
    public UserViewModel Create(INavigation navigation) =>
        new(
            logger,
            settings,
            ResolveNavigation(navigationServiceFactory, navigation),
            quotesChangeNotifier,
            profileStatsLoader,
            profileQuotesLoaderFactory(),
            practiceHistoryLoader,
            featuredTechniquesBuilder,
            quoteCommandsFactory,
            profileRefreshCoordinator,
            languageContentReloader);
}

public interface IOptionsViewModelFactory
{
    OptionsViewModel Create(INavigation navigation);
}

public sealed class OptionsViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IOptionsViewModelFactory
{
    public OptionsViewModel Create(INavigation navigation) =>
        new(ResolveNavigation(navigationServiceFactory, navigation));
}

public interface IDonateViewModelFactory
{
    DonateViewModel Create(INavigation navigation);
}

public sealed class DonateViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IDonateViewModelFactory
{
    public DonateViewModel Create(INavigation navigation) =>
        new(ResolveNavigation(navigationServiceFactory, navigation));
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
    IUserPreferencesStore userPreferencesStore,
    SettingsPreferencesPresenter settingsPreferencesPresenter,
    LanguageContentReloader languageContentReloader,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, ISettingsViewModelFactory
{
    public SettingsViewModel Create(INavigation navigation) =>
        new(
            dialogService,
            ResolveNavigation(navigationServiceFactory, navigation),
            userPreferencesStore,
            settingsPreferencesPresenter,
            languageContentReloader);
}

public interface IInfoViewModelFactory
{
    InfoViewModel Create(INavigation navigation);
}

public sealed class InfoViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IInfoViewModelFactory
{
    public InfoViewModel Create(INavigation navigation) =>
        new(ResolveNavigation(navigationServiceFactory, navigation));
}
