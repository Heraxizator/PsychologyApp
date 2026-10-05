namespace PsychologyApp.Infrastructure.Data.Context;

/// <summary>
/// Microsoft.Data.Sqlite.Core does not pick a native SQLite itself. The app uses SQLite3 Multiple Ciphers (a SQLite that can encrypt
/// the file) on every platform, so the provider is set once before the first connection is made.
/// </summary>
public static class SqliteProvider
{
    private static readonly Lock Gate = new();
    private static bool _initialized;

    public static void EnsureInitialized()
    {
        lock (Gate)
        {
            if (_initialized)
            {
                return;
            }

            SQLitePCL.Batteries_V2.Init();
            _initialized = true;
        }
    }
}
