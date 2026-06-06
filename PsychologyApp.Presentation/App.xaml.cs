using PsychologyApp.Presentation.Infrastructure;

namespace PsychologyApp.Presentation;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App(AppShell appShell)
    {
        InitializeComponent();
        UserPreferences.ApplyAll();
        GlobalExceptionHandler.Attach(this);
        MainPage = appShell;
    }
}
