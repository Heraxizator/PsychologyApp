using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Infrastructure.Data.Context;
using PsychologyApp.Infrastructure.Data.Repositories.Quots;
using PsychologyApp.Infrastructure.Data.Repositories.Statistics;
using PsychologyApp.Infrastructure.Data.Repositories.Techniques;
using PsychologyApp.Infrastructure.Data.Sql;

namespace PsychologyApp.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPsychologyAppInfrastructure(this IServiceCollection services, Action<AppSettings>? configureSettings = null)
    {
        Iso8601DateTimeHandler.Register();

        services.AddOptions<AppSettings>();
        if (configureSettings is not null)
        {
            services.Configure(configureSettings);
        }

        services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
        services.AddSingleton<IDatabaseInitializer, SqliteDatabaseInitializer>();

        services.AddScoped<ITechniqueRepository, TechniqueRepository>();
        services.AddScoped<IQuotRepository, QuotRepository>();
        services.AddScoped<IStatisticRepository, StatisticRepository>();

        return services;
    }
}
