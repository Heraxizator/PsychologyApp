using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Bootstrap;

namespace PsychologyApp.Testing.Data;

public static class IntegrationTestServiceCollection
{
    public static ServiceProvider BuildCoreProvider(
        SharedMemoryConnectionFactory connectionFactory,
        Action<ServiceCollection>? configure = null)
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddPsychologyAppCore();
        ReplaceSingleton<IDbConnectionFactory>(services, connectionFactory);
        configure?.Invoke(services);
        return services.BuildServiceProvider();
    }

    public static void ReplaceSingleton<TService>(IServiceCollection services, TService implementation)
        where TService : class
    {
        foreach (ServiceDescriptor descriptor in services.Where(d => d.ServiceType == typeof(TService)).ToList())
        {
            services.Remove(descriptor);
        }

        services.AddSingleton<TService>(implementation);
    }
}
