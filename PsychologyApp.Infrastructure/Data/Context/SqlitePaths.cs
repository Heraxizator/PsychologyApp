namespace PsychologyApp.Infrastructure.Data.Context;

public static class SqlitePaths
{
    private const string DataDirectoryName = "PsychologyApp";
    private const string DatabaseFileName = "mentalfire3.db";

    public static string GetDatabasePath()
    {
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appDataPath = Path.Join(folderPath, DataDirectoryName);
        Directory.CreateDirectory(appDataPath);
        return Path.Join(appDataPath, DatabaseFileName);
    }

    // The database is not encrypted at rest: on Android and iOS it lives in the app's private sandbox (cloud backup and device
    // transfer are excluded in data_extraction_rules.xml). An earlier "TryProtectDatabaseFile" only did anything on desktop
    // OSes the app does not ship on, and ran File.Encrypt on every connection open; it was removed rather than left as decoration.
}
