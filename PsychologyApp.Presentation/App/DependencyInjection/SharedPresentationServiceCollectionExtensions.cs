using PsychologyApp.Presentation.App.Routes;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.App.DependencyInjection;

public static class SharedPresentationServiceCollectionExtensions
{
    public static IServiceCollection AddSharedPresentation(this IServiceCollection services)
    {
        services.AddSingleton<IToastService, ToastService>();
        services.AddSingleton<IPageHost, MauiPageHost>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<GlobalExceptionHandler>();
        services.AddSingleton<IPageViewModelActivator, PageViewModelActivator>();
        services.AddSingleton<IPageFactory, PageRegistry>();
        services.AddSingleton<IShellStartupCoordinator, ShellStartupCoordinator>();
        services.AddSingleton<IUserPreferencesStore, MauiUserPreferencesStore>();
        services.AddSingleton<IDatabaseReadySignal, DatabaseReadySignal>();
        services.AddSingleton<Func<NavigationContext, INavigationService>>(sp => context =>
            context.NavigationService ?? new MauiNavigationService(
                context,
                sp.GetRequiredService<IPageFactory>(),
                sp.GetRequiredService<IShellStartupCoordinator>()));

        return services;
    }

    internal static void AddTransientFactory<T>(IServiceCollection services) where T : class
    {
        services.AddTransient<T>();
        services.AddSingleton<Func<T>>(sp => () => sp.GetRequiredService<T>());
    }
}
