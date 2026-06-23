using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.App;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App(IServiceProvider services, GlobalExceptionHandler exceptionHandler)
    {
        InitializeComponent();
        UserPreferences.ApplyAll();
        exceptionHandler.Attach(this);
        MainPage = services.GetRequiredService<AppShell>();
    }

    protected override void OnResume()
    {
        base.OnResume();
        ReduceMotion.Refresh();
    }
}
