using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.DependencyInjection;
using PsychologyApp.Infrastructure.DependencyInjection;

namespace PsychologyApp.Bootstrap;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPsychologyAppCore(
        this IServiceCollection services,
        Action<AppSettings>? configureSettings = null)
    {
        services.AddPsychologyAppInfrastructure(configureSettings);
        services.AddPsychologyAppApplication();
        return services;
    }
}
