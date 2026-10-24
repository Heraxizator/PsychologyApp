using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.App;

// SingleTop: a second launch (the launcher icon, a reminder) reuses the running activity through OnNewIntent. A second activity
// would get the same AppShell (a singleton) while the first one's Shell renderer still holds it, and the next tab tap crashed.
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        // The language may have been applied before this activity existed: its resources still carry the phone's locale.
        Shared.Common.AndroidLocale.Apply(System.Globalization.CultureInfo.CurrentUICulture.Name);
        HandleReminderIntent(Intent);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        HandleReminderIntent(intent);
    }

    private static void HandleReminderIntent(Intent? intent)
    {
        if (intent is null || !IsFromOurApp(intent))
        {
            return;
        }

        if (string.Equals(intent.Action, QuoteReminderConstants.ActionOpenFromNotification, StringComparison.Ordinal))
        {
            QuoteReminderTapHandler.Handle();
            return;
        }

        if (string.Equals(intent.Action, MoodReminderConstants.ActionOpenFromNotification, StringComparison.Ordinal))
        {
            MoodReminderTapHandler.Handle();
            return;
        }

        if (string.Equals(intent.Action, ChatReminderConstants.ActionOpenFromNotification, StringComparison.Ordinal))
        {
            ChatReminderTapHandler.Handle();
            return;
        }

        if (string.Equals(intent.Action, MoodWidgetConstants.ActionMood, StringComparison.Ordinal))
        {
            MoodWidgetTapHandler.HandleMood(intent.GetIntExtra(MoodWidgetConstants.ExtraMoodLevel, 0));
            return;
        }

        if (string.Equals(intent.Action, MoodWidgetConstants.ActionPractice, StringComparison.Ordinal))
        {
            MoodWidgetTapHandler.HandlePractice();
            return;
        }

        if (!string.Equals(intent.Action, PracticeReminderConstants.ActionOpenFromNotification, StringComparison.Ordinal))
        {
            return;
        }

        string? techniqueValue = intent.GetStringExtra(PracticeReminderConstants.ExtraTechniqueId);
        if (Enum.TryParse(techniqueValue, out TechniqueId techniqueId))
        {
            PracticeReminderTapHandler.Handle(techniqueId);
        }
    }

    // Package and action are chosen by the sender; only our own notifications carry the per-install secret (see ReminderIntentToken).
    private static bool IsFromOurApp(Intent intent) => PsychologyApp.Presentation.Platforms.Android.ReminderIntentToken.IsValid(intent);
}
