using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Startup;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.ReasonSearch;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Application.Services.Statistic;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
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
        services.AddSingleton<ITechniqueService, TechniqueService>();
        services.AddSingleton<IQuotService, QuotService>();
        services.AddSingleton<IReasonService, ReasonService>();
        services.AddSingleton<IReasonSearchService, ReasonSearchService>();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IStatisticService, StatisticService>();
        services.AddSingleton<IUserProgressService, UserProgressService>();
        services.AddSingleton<IPageAnalyticsService, PageAnalyticsService>();
        services.AddSingleton<IAppStartupService, AppStartupService>();

        return services;
    }
}
