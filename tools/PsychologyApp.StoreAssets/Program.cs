using System.Text.Json;
using PsychologyApp.Application.DataBackup;

namespace PsychologyApp.StoreAssets;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length >= 2 && args[0] == "demo")
        {
            string json = JsonSerializer.Serialize(DemoBackup.Build(DateTime.UtcNow), BackupJsonContext.Default.AppBackupDTO);
            File.WriteAllText(args[1], json);
            Console.WriteLine($"Demo backup written to {args[1]} ({json.Length} characters).");
            return 0;
        }

        if (args.Length >= 3 && args[0] == "compose")
        {
            StoreImages.Compose(args[1], args[2]);
            return 0;
        }

        Console.WriteLine("Usage: StoreAssets demo <backup.json> | compose <rawDir> <outDir>");
        return 1;
    }
}
