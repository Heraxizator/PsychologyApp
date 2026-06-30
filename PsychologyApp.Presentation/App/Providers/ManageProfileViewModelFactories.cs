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
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Pages.ProfileUser;

namespace PsychologyApp.Presentation.App.Providers;

public interface IUserViewModelFactory
{
    UserViewModel Create(ContentPage page);
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
    public UserViewModel Create(ContentPage page) =>
        new(
            logger,
            settings,
            ResolveNavigation(navigationServiceFactory, page),
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
    OptionsViewModel Create(ContentPage page);
}

public sealed class OptionsViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IOptionsViewModelFactory
{
    public OptionsViewModel Create(ContentPage page) =>
        new(ResolveNavigation(navigationServiceFactory, page));
}

public interface IDonateViewModelFactory
{
    DonateViewModel Create(ContentPage page);
}

public sealed class DonateViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IDonateViewModelFactory
{
    public DonateViewModel Create(ContentPage page) =>
        new(ResolveNavigation(navigationServiceFactory, page));
}

public interface ISettingsViewModelFactory
{
    SettingsViewModel Create(ContentPage page);
}

public sealed class SettingsViewModelFactory(
    IDialogService dialogService,
    IUserPreferencesStore userPreferencesStore,
    SettingsPreferencesPresenter settingsPreferencesPresenter,
    LanguageContentReloader languageContentReloader,
    IPracticeReminderCoordinator practiceReminderCoordinator,
    Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, ISettingsViewModelFactory
{
    public SettingsViewModel Create(ContentPage page) =>
        new(
            dialogService,
            ResolveNavigation(navigationServiceFactory, page),
            userPreferencesStore,
            settingsPreferencesPresenter,
            languageContentReloader,
            practiceReminderCoordinator);
}

public interface IInfoViewModelFactory
{
    InfoViewModel Create(ContentPage page);
}

public sealed class InfoViewModelFactory(Func<NavigationContext, INavigationService> navigationServiceFactory) : ViewModelFactoryBase, IInfoViewModelFactory
{
    public InfoViewModel Create(ContentPage page) =>
        new(ResolveNavigation(navigationServiceFactory, page));
}
