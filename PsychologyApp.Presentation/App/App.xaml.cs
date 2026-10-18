using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Lib.Navigation;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Features.ManageJournal;

namespace PsychologyApp.Presentation.App;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services, GlobalExceptionHandler exceptionHandler)
    {
        _services = services;
        InitializeComponent();
        UserPreferences.ApplyAll();
        exceptionHandler.Attach(this);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        AppShell shell = _services.GetRequiredService<AppShell>();
        IShellTabNavigator tabNavigator = _services.GetRequiredService<IShellTabNavigator>();
        IUserPreferencesStore preferences = _services.GetRequiredService<IUserPreferencesStore>();
        PracticeReminderTapHandler.Configure(tabNavigator, preferences);
        QuoteReminderTapHandler.Configure(tabNavigator);
        MoodReminderTapHandler.Configure(tabNavigator, preferences);
        ChatReminderTapHandler.Configure(tabNavigator);
        MoodWidgetTapHandler.Configure(
            tabNavigator,
            _services.GetRequiredService<JournalMoodLoader>(),
            _services.GetRequiredService<IDatabaseReadySignal>());
#if DEBUG
        Shared.Common.Infrastructure.DemoDataImporter.TryImportAsync(_services).FireAndForget();
#endif
        return new(shell);
    }

    protected override void OnResume()
    {
        base.OnResume();
        ReduceMotion.Refresh();
        _services.GetRequiredService<IPracticeReminderCoordinator>().SyncAsync().FireAndForget();
        _services.GetRequiredService<IQuoteReminderCoordinator>().SyncAsync().FireAndForget();
        _services.GetRequiredService<IMoodReminderCoordinator>().SyncAsync().FireAndForget();
        _services.GetRequiredService<IChatReminderCoordinator>().SyncAsync().FireAndForget();
    }

    // A check-in made in the app (journal, after a practice, from the widget) moves today's mood reminder to tomorrow.
    protected override void OnSleep()
    {
        base.OnSleep();
        _services.GetRequiredService<IMoodReminderCoordinator>().SyncAsync().FireAndForget();
#if ANDROID
        // The widget's texts follow the app language; a switch made in this session reaches it here.
        Platforms.Android.MoodWidgetProvider.Refresh(Platform.AppContext);
#endif
    }
}
