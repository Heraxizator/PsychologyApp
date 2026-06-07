using PsychologyApp.Presentation.Infrastructure;

namespace PsychologyApp.Presentation;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App(IServiceProvider services)
    {
        InitializeComponent();
        UserPreferences.ApplyAll();
        GlobalExceptionHandler.Attach(this);
        MainPage = services.GetRequiredService<AppShell>();
    }
}
