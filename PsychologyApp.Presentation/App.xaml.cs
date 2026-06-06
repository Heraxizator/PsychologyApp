using PsychologyApp.Presentation.Infrastructure;

namespace PsychologyApp.Presentation;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App(AppShell appShell)
    {
        InitializeComponent();
        UserPreferences.ApplyTheme();
        GlobalExceptionHandler.Attach(this);
        MainPage = appShell;
    }
}
