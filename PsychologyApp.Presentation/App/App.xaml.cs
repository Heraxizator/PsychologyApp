using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Notifications;

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
        var tabNavigator = new AppShellTabNavigator(shell);
        PracticeReminderTapHandler.Configure(tabNavigator);
        QuoteReminderTapHandler.Configure(tabNavigator);
        MoodReminderTapHandler.Configure(tabNavigator);
        return new(shell);
    }

    protected override void OnResume()
    {
        base.OnResume();
        ReduceMotion.Refresh();
        _services.GetRequiredService<IPracticeReminderCoordinator>().SyncAsync().FireAndForget();
        _services.GetRequiredService<IQuoteReminderCoordinator>().SyncAsync().FireAndForget();
        _services.GetRequiredService<IMoodReminderCoordinator>().SyncAsync().FireAndForget();
    }
}
