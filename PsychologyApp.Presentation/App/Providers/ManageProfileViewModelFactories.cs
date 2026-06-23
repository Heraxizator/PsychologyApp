using PsychologyApp.Presentation.Pages.ProfileSettings;
using PsychologyApp.Presentation.Pages.ProfileDonate;
using PsychologyApp.Presentation.Pages.ProfileInfo;
using PsychologyApp.Presentation.Pages.ProfileOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Pages.ProfileUser;

namespace PsychologyApp.Presentation.App.Providers;

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
