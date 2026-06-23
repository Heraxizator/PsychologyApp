using Foundation;

using PsychologyApp.Presentation.App;

namespace PsychologyApp.Presentation.App;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
