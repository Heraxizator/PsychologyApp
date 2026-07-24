#if ANDROID
using PsychologyApp.Presentation.Platforms.Android;
#endif
using PsychologyApp.Presentation.App.Routes;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Common.Localization;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using PsychologyApp.Presentation.Shared.Services.Clipboard;
using PsychologyApp.Presentation.Shared.Services.Notifications;
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
        services.AddSingleton<IAppClipboardService, AppClipboardService>();
        services.AddSingleton<IPageHost, MauiPageHost>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<GlobalExceptionHandler>();
        services.AddSingleton<IPageViewModelActivator, PageViewModelActivator>();
        services.AddSingleton<IPageFactory, PageRegistry>();
        services.AddSingleton<IShellStartupCoordinator, ShellStartupCoordinator>();
        services.AddSingleton<IUserPreferencesStore, MauiUserPreferencesStore>();
        services.AddSingleton<IPracticeReminderCoordinator, PracticeReminderCoordinator>();
        services.AddSingleton<IQuoteReminderCoordinator, QuoteReminderCoordinator>();
        services.AddSingleton<IMoodReminderCoordinator, MoodReminderCoordinator>();
#if ANDROID
        services.AddSingleton<IPracticeReminderScheduler, AndroidPracticeReminderScheduler>();
        services.AddSingleton<IQuoteReminderScheduler, AndroidQuoteReminderScheduler>();
        services.AddSingleton<IMoodReminderScheduler, AndroidMoodReminderScheduler>();
#else
        services.AddSingleton<IPracticeReminderScheduler, NullPracticeReminderScheduler>();
        services.AddSingleton<IQuoteReminderScheduler, NullQuoteReminderScheduler>();
        services.AddSingleton<IMoodReminderScheduler, NullMoodReminderScheduler>();
#endif
        services.AddSingleton<IDatabaseReadySignal, DatabaseReadySignal>();
        services.AddSingleton<LanguageContentReloader>();
        services.AddSingleton<PsychologyApp.Presentation.Shared.Services.Progress.WeeklyInsightLoader>();
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
