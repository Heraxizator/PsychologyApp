using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.ManageProfile.Index;
namespace PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;

public static class ManageProfileFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddManageProfileFeature(this IServiceCollection services)
    {
        services.AddFeatureSingleton<IProfilePageFactory, ProfilePageFactory>();
        services.AddFeatureSingleton<ProfileStatsLoader>();
        services.AddFeatureSingleton<ProfileQuotesPresenter>();
        SharedPresentationServiceCollectionExtensions.AddTransientFactory<ProfileQuotesLoader>(services);
        services.AddFeatureSingleton<SettingsPreferencesPresenter>();
        services.AddFeatureSingleton<PracticeHistoryFormatter>();
        services.AddFeatureSingleton<ProfilePracticeHistoryLoader>();
        services.AddFeatureSingleton<ProfileFeaturedTechniquesBuilder>();
        services.AddFeatureSingleton<UserProfileRefreshCoordinator>();
        services.AddFeatureSingleton<ProfileScreenCoordinator>();
        services.AddFeatureViewModelFactory<IUserViewModelFactory, UserViewModelFactory>();
        services.AddFeatureViewModelFactory<IOptionsViewModelFactory, OptionsViewModelFactory>();
        services.AddFeatureViewModelFactory<IInfoViewModelFactory, InfoViewModelFactory>();
        services.AddFeatureViewModelFactory<IDonateViewModelFactory, DonateViewModelFactory>();
        services.AddFeatureViewModelFactory<ISettingsViewModelFactory, SettingsViewModelFactory>();
        return services;
    }
}
