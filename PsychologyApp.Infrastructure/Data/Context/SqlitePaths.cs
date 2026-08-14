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

    public static void TryProtectDatabaseFile(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        try
        {
            if (OperatingSystem.IsWindows())
            {
                File.Encrypt(path);
                return;
            }

            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                File.SetUnixFileMode(
                    path,
                    UnixFileMode.UserRead | UnixFileMode.UserWrite);
            }
        }
        catch
        {
            // Best-effort hardening. App keeps running even if file protection fails.
        }
    }
}
