using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Startup;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Application.Services.Statistic;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Startup;
using PsychologyApp.Application.Analytics;

namespace PsychologyApp.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCachedReasonContentProvider<TInner>(this IServiceCollection services)
        where TInner : class, IReasonContentProvider
    {
        services.AddSingleton<CachedReasonContentProvider>(sp =>
            new CachedReasonContentProvider(sp.GetRequiredService<TInner>()));
        services.AddSingleton<IReasonContentProvider>(sp => sp.GetRequiredService<CachedReasonContentProvider>());

        return services;
    }

    public static IServiceCollection AddCachedQuotContentProvider<TInner>(this IServiceCollection services)
        where TInner : class, IQuotContentProvider
    {
        services.AddSingleton<CachedQuotContentProvider>(sp =>
            new CachedQuotContentProvider(sp.GetRequiredService<TInner>()));
        services.AddSingleton<IQuotContentProvider>(sp => sp.GetRequiredService<CachedQuotContentProvider>());

        return services;
    }

    public static IServiceCollection AddPsychologyAppApplication(this IServiceCollection services)
    {
        services.AddScoped<ITechniqueService, TechniqueService>();
        services.AddScoped<IQuotService, QuotService>();
        services.AddScoped<IReasonService, ReasonService>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IStatisticService, StatisticService>();
        services.AddScoped<IPageAnalyticsService, PageAnalyticsService>();
        services.AddScoped<IAppStartupService, AppStartupService>();

        return services;
    }
}
