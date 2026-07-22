using PsychologyApp.Presentation.App.DependencyInjection;
using PsychologyApp.Presentation.Features.ClinicalCare;

namespace PsychologyApp.Presentation.Features.ClinicalCare.DependencyInjection;

public static class ClinicalCareFeatureServiceCollectionExtensions
{
    public static IServiceCollection AddClinicalCareFeature(this IServiceCollection services)
    {
        services.AddFeatureSingleton<ICrisisHubViewModelFactory, CrisisHubViewModelFactory>();
        services.AddFeatureSingleton<IRiskCheckViewModelFactory, RiskCheckViewModelFactory>();
        services.AddFeatureSingleton<IClinicalCarePageFactory, ClinicalCarePageFactory>();
        return services;
    }
}
