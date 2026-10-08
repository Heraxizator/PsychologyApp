#if DEBUG
using PsychologyApp.Application.DataBackup;

namespace PsychologyApp.Presentation.Shared.Common.Infrastructure;

/// <summary>
/// Debug builds only: fills the app with a demo month of use for store screenshots and manual checks. If a file named <c>demo-backup.json</c> (made by
/// <c>tools/PsychologyApp.StoreAssets</c>) lies in the app's data folder at start, it is imported once through the ordinary backup import and renamed.
/// Release builds do not contain this code.
/// </summary>
internal static class DemoDataImporter
{
    private const string FileName = "demo-backup.json";

    public static async Task TryImportAsync(IServiceProvider services)
    {
        string path = Path.Combine(FileSystem.AppDataDirectory, FileName);
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            string json = await File.ReadAllTextAsync(path).ConfigureAwait(false);
            BackupImportResult result = await services.GetRequiredService<IBackupService>().ImportAsync(json).ConfigureAwait(false);
            File.Move(path, path + ".done", overwrite: true);
            System.Diagnostics.Debug.WriteLine($"Demo data imported: {result.MoodEntries} moods, {result.Completions} practices, {result.ChatSessions} chats.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Demo data import failed: {ex}");
        }
    }
}
#endif
