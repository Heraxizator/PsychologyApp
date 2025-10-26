using Android.App;
using Android.Content.PM;
using Android.OS;

namespace PsychologyApp.Presentation;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public override void OnCreate(Bundle? savedInstanceState, PersistableBundle? persistentState)
    {
        base.OnCreate(savedInstanceState, persistentState);

        var uiModeManager = (UiModeManager)GetSystemService(UiModeService);
        uiModeManager.SetApplicationNightMode(1);
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var uiModeManager = (UiModeManager)GetSystemService(UiModeService);
        uiModeManager.SetApplicationNightMode(1);
    }
}
