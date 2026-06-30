using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        HandleReminderIntent(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        HandleReminderIntent(intent);
    }

    private static void HandleReminderIntent(Intent? intent)
    {
        if (intent?.HasExtra(PracticeReminderConstants.ExtraTechniqueId) != true)
        {
            return;
        }

        string? techniqueValue = intent.GetStringExtra(PracticeReminderConstants.ExtraTechniqueId);
        if (Enum.TryParse(techniqueValue, out TechniqueId techniqueId))
        {
            PracticeReminderTapHandler.Handle(techniqueId);
        }
    }
}
