using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App(IServiceProvider services, GlobalExceptionHandler exceptionHandler)
    {
        InitializeComponent();
        UserPreferences.ApplyAll();
        exceptionHandler.Attach(this);
        MainPage = services.GetRequiredService<AppShell>();
    }
}
