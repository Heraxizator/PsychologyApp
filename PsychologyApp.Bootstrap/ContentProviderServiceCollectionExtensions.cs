using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.DependencyInjection;
using PsychologyApp.Application.Reason;

namespace PsychologyApp.Bootstrap;

public static class ContentProviderServiceCollectionExtensions
{
    public static IServiceCollection AddPsychologyAppCachedReasonContent(
        this IServiceCollection services,
        Func<IServiceProvider, CachedReasonContentProvider> factory)
    {
        services.AddSingleton(factory);
        services.AddSingleton<IReasonContentProvider>(sp => sp.GetRequiredService<CachedReasonContentProvider>());
        return services;
    }

    public static IServiceCollection AddPsychologyAppCachedQuotContent<TInner>(this IServiceCollection services)
        where TInner : class, IQuotContentProvider
    {
        services.AddCachedQuotContentProvider<TInner>();
        return services;
    }
}
