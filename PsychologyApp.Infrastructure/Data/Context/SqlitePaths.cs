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

    // The file is encrypted at rest (SqliteEncryption, with the key from IDatabaseKeyProvider) and lives in the app's private sandbox;
    // cloud backup and device transfer are excluded in data_extraction_rules.xml.
}
