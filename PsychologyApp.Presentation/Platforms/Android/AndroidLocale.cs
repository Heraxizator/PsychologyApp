using Android.Content.Res;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// System dialogs (the picker's Cancel button, date and time pickers) read Android's own strings, which follow the context's
/// configuration, not .NET's culture: in German the settings picker said "Cancel". The configuration is updated in place;
/// the per-app locale API would recreate the activity and lose the navigation stack.
/// </summary>
internal static class AndroidLocale
{
    public static void Apply(string cultureName)
    {
        Java.Util.Locale locale = Java.Util.Locale.ForLanguageTag(cultureName);
        Java.Util.Locale.Default = locale;
        foreach (Android.Content.Context? context in new Android.Content.Context?[] { Microsoft.Maui.ApplicationModel.Platform.CurrentActivity, Android.App.Application.Context })
        {
            if (context?.Resources is not { Configuration: { } current } resources)
            {
                continue;
            }

            Configuration configuration = new(current);
            configuration.SetLocale(locale);
#pragma warning disable CA1422 // Deprecated, but createConfigurationContext returns a new context that the open activity's dialogs would not use.
            resources.UpdateConfiguration(configuration, resources.DisplayMetrics);
#pragma warning restore CA1422
        }
    }
}
