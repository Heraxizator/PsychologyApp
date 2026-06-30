using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Features.ManageProfile;

namespace PsychologyApp.Presentation.App.Providers;

public static class ManageProfileFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddManageProfileFeature(this IServiceCollection services)
    {
        services.AddSingleton<IProfilePageFactory, ProfilePageFactory>();
        services.AddSingleton<ProfileStatsLoader>();
        services.AddSingleton<ProfileQuotesPresenter>();
        SharedPresentationServiceCollectionExtensions.AddTransientFactory<ProfileQuotesLoader>(services);
        services.AddSingleton<SettingsPreferencesPresenter>();
        services.AddSingleton<PracticeHistoryFormatter>();
        services.AddSingleton<ProfilePracticeHistoryLoader>();
        services.AddSingleton<ProfileFeaturedTechniquesBuilder>();
        services.AddSingleton<UserProfileRefreshCoordinator>();
        services.AddSingleton<IUserViewModelFactory, UserViewModelFactory>();
        services.AddSingleton<IOptionsViewModelFactory, OptionsViewModelFactory>();
        services.AddSingleton<IInfoViewModelFactory, InfoViewModelFactory>();
        services.AddSingleton<IDonateViewModelFactory, DonateViewModelFactory>();
        services.AddSingleton<ISettingsViewModelFactory, SettingsViewModelFactory>();
        return services;
    }
}
