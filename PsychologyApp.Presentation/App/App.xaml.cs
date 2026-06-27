using PsychologyApp.Presentation.Shared.Common;

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

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(_services.GetRequiredService<AppShell>());

    protected override void OnResume()
    {
        base.OnResume();
        ReduceMotion.Refresh();
    }
}
