namespace PsychologyApp.Application.DataBackup;

public interface IBackupService
{
    Task<string> ExportAsync(CancellationToken cancellationToken = default);
    Task<BackupImportResult> ImportAsync(string json, CancellationToken cancellationToken = default);

    /// <summary>The same content as <see cref="ExportAsync"/>, encrypted with a passphrase (at least 8 characters).</summary>
    Task<string> ExportEncryptedAsync(string passphrase, CancellationToken cancellationToken = default);

    /// <summary>Opens an encrypted backup file; throws <see cref="BackupPassphraseException"/> for a wrong passphrase.</summary>
    Task<BackupImportResult> ImportEncryptedAsync(string file, string passphrase, CancellationToken cancellationToken = default);

    /// <summary>True when the file needs a passphrase before it can be imported.</summary>
    bool IsEncrypted(string file);
}
