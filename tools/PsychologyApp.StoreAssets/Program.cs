using System.Text.Json;
using PsychologyApp.Application.DataBackup;

namespace PsychologyApp.StoreAssets;

/// <summary>Writes the demo backup for store screenshots; the slides themselves are made by tools/store-screenshots/compose.mjs.</summary>
public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length >= 2 && args[0] == "demo")
        {
            string json = JsonSerializer.Serialize(DemoBackup.Build(
                DateTime.UtcNow,
                english: args.Contains("en"),
                args.FirstOrDefault(arg => arg.StartsWith("tz=", StringComparison.Ordinal)) is { } tz ? TimeZoneInfo.FindSystemTimeZoneById(tz[3..]) : TimeZoneInfo.Local), BackupJsonContext.Default.AppBackupDTO);
            File.WriteAllText(args[1], json);
            Console.WriteLine($"Demo backup written to {args[1]} ({json.Length} characters).");
            return 0;
        }

        Console.WriteLine("Usage: StoreAssets demo <backup.json> [en] [tz=<phone time zone id, e.g. UTC>]");
        return 1;
    }
}
