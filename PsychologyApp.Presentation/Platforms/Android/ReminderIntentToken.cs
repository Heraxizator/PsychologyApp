#if ANDROID
using Android.Content;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>
/// MainActivity is exported (it is the launcher), so any app can send it an intent with a reminder's action and our package name;
/// those two are not proof of origin. The tap intents of our own notifications also carry a random per-install secret that no other
/// app can know, and only an intent with it is treated as a reminder tap.
/// </summary>
internal static class ReminderIntentToken
{
    private const string ExtraName = "psychologyapp.reminder_token";
    private const string PreferenceKey = "ReminderIntentToken";

    private static string Token
    {
        get
        {
            string? token = Preferences.Default.Get<string?>(PreferenceKey, null);
            if (string.IsNullOrEmpty(token))
            {
                token = Guid.NewGuid().ToString("N");
                Preferences.Default.Set(PreferenceKey, token);
            }

            return token;
        }
    }

    internal static void Put(Intent intent) => intent.PutExtra(ExtraName, Token);

    internal static bool IsValid(Intent intent) =>
        string.Equals(intent.GetStringExtra(ExtraName), Token, StringComparison.Ordinal);
}
#endif
