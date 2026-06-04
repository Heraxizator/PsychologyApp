namespace PsychologyApp.Infrastructure.Data.Context;

public static class SqlitePaths
{
    public static string GetDatabasePath()
    {
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Join(folderPath, "mentalfire3.db");
    }
}
