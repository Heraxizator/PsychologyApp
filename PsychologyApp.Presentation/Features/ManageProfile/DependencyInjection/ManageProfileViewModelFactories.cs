using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileDonate;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileInfo;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.ManageProfile.Index;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Features.ManageQuotes.Index;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;
using PsychologyApp.Presentation.Shared.Services.Toasts;

namespace PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;

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
    ProfileScreenCoordinator profileScreenCoordinator,
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
            profileScreenCoordinator,
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
