using Foundation;

using PsychologyApp.Presentation.App;

namespace PsychologyApp.Presentation
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
