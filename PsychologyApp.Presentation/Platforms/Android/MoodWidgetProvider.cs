#if ANDROID
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.ManageJournal;

namespace PsychologyApp.Presentation.Platforms.Android;

/// <summary>
/// Home-screen widget: five mood faces and "practice of the day". It shows nothing personal (no mood history, no practice
/// name), since a home screen is seen by others. Each tap opens the app through MainActivity with the per-install token.
/// </summary>
[BroadcastReceiver(Label = "@string/mood_widget_label", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetUpdate])]
[MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/mood_widget_info")]
public sealed class MoodWidgetProvider : AppWidgetProvider
{
    private const int RequestCodeBase = 9100;

    private static readonly int[] FaceIds =
    [
        Resource.Id.mood_widget_1,
        Resource.Id.mood_widget_2,
        Resource.Id.mood_widget_3,
        Resource.Id.mood_widget_4,
        Resource.Id.mood_widget_5
    ];

    public override void OnUpdate(Context? context, AppWidgetManager? appWidgetManager, int[]? appWidgetIds)
    {
        if (context is null || appWidgetManager is null || appWidgetIds is null)
        {
            return;
        }

        appWidgetManager.UpdateAppWidget(appWidgetIds, Build(context));
    }

    /// <summary>Redraws placed widgets, so their texts follow a language switch in the app.</summary>
    internal static void Refresh(Context context)
    {
        AppWidgetManager? manager = AppWidgetManager.GetInstance(context);
        int[]? ids = manager?.GetAppWidgetIds(new ComponentName(context, Java.Lang.Class.FromType(typeof(MoodWidgetProvider))));
        if (manager is not null && ids is { Length: > 0 })
        {
            manager.UpdateAppWidget(ids, Build(context));
        }
    }

    private static RemoteViews Build(Context context)
    {
        // A widget update can start the process without the app window, where the language provider is not set yet.
        if (AppStrings.LanguageProvider is null)
        {
            AppStrings.LanguageProvider = UserPreferences.GetPersistedLanguage;
        }

        RemoteViews views = new(context.PackageName, Resource.Layout.mood_widget);
        views.SetTextViewText(Resource.Id.mood_widget_title, AppStrings.MoodWidgetTitle);
        for (int level = MoodWidgetConstants.MinMoodLevel; level <= MoodWidgetConstants.MaxMoodLevel; level++)
        {
            int faceId = FaceIds[level - 1];
            views.SetTextViewText(faceId, AppStrings.MoodEmojiFor(level));
            views.SetContentDescription(faceId, AppStrings.MoodWidgetLevel(level));
            views.SetOnClickPendingIntent(faceId, OpenApp(context, MoodWidgetConstants.ActionMood, level));
        }

        views.SetTextViewText(Resource.Id.mood_widget_practice, $"{AppStrings.MoodWidgetPractice} →");
        views.SetContentDescription(Resource.Id.mood_widget_practice, AppStrings.MoodWidgetPractice);
        views.SetOnClickPendingIntent(Resource.Id.mood_widget_practice, OpenApp(context, MoodWidgetConstants.ActionPractice, 0));
        return views;
    }

    private static PendingIntent OpenApp(Context context, string action, int moodLevel)
    {
        Intent intent = new(context, typeof(global::PsychologyApp.Presentation.App.MainActivity));
        intent.SetAction(action);
        intent.SetPackage(context.PackageName);
        intent.PutExtra(MoodWidgetConstants.ExtraMoodLevel, moodLevel);
        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);
        ReminderIntentToken.Put(intent);

        // One request code per button: PendingIntents that differ only in extras would replace each other.
        return PendingIntent.GetActivity(
            context,
            RequestCodeBase + moodLevel,
            intent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent)!;
    }
}
#endif
