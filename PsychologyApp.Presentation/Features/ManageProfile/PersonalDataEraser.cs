using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Abstractions.Persistence;

namespace PsychologyApp.Presentation.Features.ManageProfile;

/// <summary>
/// The database file is replaced by an empty one (journal, practices, tests, chats, safety plan, body log, drafts, saved
/// quotes), settings are cleared, the logs are deleted, and so are the app's own files in the cache: an exported journal,
/// backup, summary or error log waiting to be shared, and a picked backup the file picker copied there. The rest of the cache is
/// left alone: it holds the icon fonts MAUI unpacks there, the system's compiled code and downloaded music, none of it the
/// person's. The database key stays: it protects the new, empty file just as well. The app then restarts, so no screen or
/// service keeps an old copy in memory.
/// </summary>
public sealed class PersonalDataEraser(IDatabaseInitializer database, ILogger<PersonalDataEraser> logger) : IPersonalDataEraser
{
    // The rest runs while the database is held for this alone: the restart ends the process there, so a save that was already
    // queued (a note being autosaved, a chat turn) never reaches the new, empty database.
    public Task EraseAllAsync(CancellationToken cancellationToken = default) =>
        database.RecreateDatabaseAsync(() =>
        {
            DeleteFilesAndSettings();
            Restart();
            return Task.CompletedTask;
        }, cancellationToken);

    private void DeleteFilesAndSettings()
    {
        DeleteQuietly(Path.Combine(FileSystem.AppDataDirectory, "logs"));
        if (Directory.Exists(FileSystem.CacheDirectory))
        {
            foreach (string file in Directory.EnumerateFiles(FileSystem.CacheDirectory, "*", SearchOption.AllDirectories))
            {
                if (SharedFileNames.IsOwn(Path.GetFileName(file)))
                {
                    DeleteQuietly(file);
                }
            }
        }

        ClearPreferences();
        logger.LogInformation("All personal data was deleted; restarting");
    }

    // A file that cannot be removed must not stop the rest: the database, which holds what matters, is already gone.
    private void DeleteQuietly(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(ex, "A file could not be deleted while erasing the data");
        }
    }

    /// <summary>
    /// On Android, Preferences.Clear writes to disk in the background; the restart ends the process at once, before that write
    /// lands, and the old settings came back (onboarding counted as done). The file MAUI uses is cleared and committed here instead.
    /// </summary>
    private static void ClearPreferences()
    {
#if ANDROID
        global::Android.Content.Context context = Platform.AppContext;
        context.GetSharedPreferences($"{context.PackageName}_preferences", global::Android.Content.FileCreationMode.Private)?
            .Edit()?
            .Clear()?
            .Commit();
#else
        Preferences.Default.Clear();
#endif
    }

    private static void Restart()
    {
#if ANDROID
        // The new launch is asked for while the app is still in front, then this process ends: Android starts the app in a
        // fresh process, with nothing of the old state left in memory.
        global::Android.App.Activity? activity = Platform.CurrentActivity;
        if (activity?.PackageManager?.GetLaunchIntentForPackage(activity.PackageName!) is { } launch)
        {
            launch.AddFlags(global::Android.Content.ActivityFlags.NewTask | global::Android.Content.ActivityFlags.ClearTask);
            activity.StartActivity(launch);
        }

        Java.Lang.JavaSystem.Exit(0);
#endif
    }
}
