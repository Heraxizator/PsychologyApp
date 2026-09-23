namespace PsychologyApp.Application.DataBackup;

public interface IBackupService
{
    Task<string> ExportAsync(CancellationToken cancellationToken = default);
    Task<BackupImportResult> ImportAsync(string json, CancellationToken cancellationToken = default);
}
