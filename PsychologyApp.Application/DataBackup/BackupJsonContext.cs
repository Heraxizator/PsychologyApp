using System.Text.Json.Serialization;

namespace PsychologyApp.Application.DataBackup;

/// <summary>Source-generated so that export and import survive trimming and AOT in release builds (no reflection over the DTOs).</summary>
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppBackupDTO))]
public partial class BackupJsonContext : JsonSerializerContext;
