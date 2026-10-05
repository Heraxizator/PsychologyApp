using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PsychologyApp.Application.DataBackup;

/// <summary>The file written for an encrypted backup: everything needed to decrypt it except the passphrase.</summary>
public sealed class EncryptedBackupEnvelope
{
    public string Format { get; set; } = string.Empty;
    public int Version { get; set; }
    public string Kdf { get; set; } = string.Empty;
    public int Iterations { get; set; }
    public string Salt { get; set; } = string.Empty;
    public string Nonce { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
}

/// <summary>The passphrase is missing or wrong, or the file was changed after it was written.</summary>
public sealed class BackupPassphraseException : Exceptions.AppException
{
    public BackupPassphraseException(string message) : base(message)
    {
    }
}

/// <summary>
/// Passphrase protection for backup files: PBKDF2-SHA256 (high iteration count) derives a 256-bit key, AES-256-GCM encrypts and
/// authenticates. A backup holds chats, mood notes and the safety plan, so a file that leaves the device (a share sheet, a mail
/// attachment, a cloud folder) should not be readable by whoever finds it.
/// </summary>
public static class BackupEncryption
{
    public const string FormatName = "psychologyapp-backup-encrypted";
    public const int MinPassphraseLength = 8;

    private const int CurrentVersion = 1;
    private const int KdfIterations = 600_000;
    private const int SaltBytes = 16;
    private const int NonceBytes = 12;
    private const int TagBytes = 16;
    private const int KeyBytes = 32;

    public static bool IsEncrypted(string file)
    {
        // Cheap check first: an encrypted file is a small JSON object that names its format at the top.
        if (string.IsNullOrWhiteSpace(file) || !file.Contains(FormatName, StringComparison.Ordinal))
        {
            return false;
        }

        try
        {
            EncryptedBackupEnvelope? envelope = JsonSerializer.Deserialize(file, BackupJsonContext.Default.EncryptedBackupEnvelope);
            return envelope is not null && envelope.Format == FormatName;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static string Encrypt(string plainJson, string passphrase)
    {
        if (string.IsNullOrEmpty(passphrase) || passphrase.Length < MinPassphraseLength)
        {
            throw new ArgumentException($"The passphrase must have at least {MinPassphraseLength} characters.", nameof(passphrase));
        }

        byte[] salt = RandomNumberGenerator.GetBytes(SaltBytes);
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceBytes);
        byte[] key = DeriveKey(passphrase, salt, KdfIterations);
        byte[] plain = Encoding.UTF8.GetBytes(plainJson);
        byte[] cipher = new byte[plain.Length];
        byte[] tag = new byte[TagBytes];
        try
        {
            using AesGcm aes = new(key, TagBytes);
            aes.Encrypt(nonce, plain, cipher, tag, AssociatedData(CurrentVersion));
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plain);
        }

        EncryptedBackupEnvelope envelope = new()
        {
            Format = FormatName,
            Version = CurrentVersion,
            Kdf = "PBKDF2-SHA256",
            Iterations = KdfIterations,
            Salt = Convert.ToBase64String(salt),
            Nonce = Convert.ToBase64String(nonce),
            Tag = Convert.ToBase64String(tag),
            Data = Convert.ToBase64String(cipher)
        };
        return JsonSerializer.Serialize(envelope, BackupJsonContext.Default.EncryptedBackupEnvelope);
    }

    public static string Decrypt(string file, string passphrase)
    {
        EncryptedBackupEnvelope envelope;
        try
        {
            envelope = JsonSerializer.Deserialize(file, BackupJsonContext.Default.EncryptedBackupEnvelope)
                ?? throw new BackupFormatException("The file is not an encrypted backup.");
        }
        catch (JsonException ex)
        {
            throw new BackupFormatException("The file is not an encrypted backup.", ex);
        }

        if (envelope.Format != FormatName || envelope.Kdf != "PBKDF2-SHA256")
        {
            throw new BackupFormatException("The file is not an encrypted backup.");
        }

        if (envelope.Version > CurrentVersion)
        {
            throw new BackupFormatException("The backup was made by a newer version of the app.");
        }

        // A hostile file could ask for billions of iterations; only a sane range is honoured.
        if (envelope.Iterations is < 100_000 or > 5_000_000)
        {
            throw new BackupFormatException("The encrypted backup has unsupported settings.");
        }

        if (string.IsNullOrEmpty(passphrase))
        {
            throw new BackupPassphraseException("A passphrase is needed to open this backup.");
        }

        byte[] salt, nonce, tag, cipher;
        try
        {
            salt = Convert.FromBase64String(envelope.Salt);
            nonce = Convert.FromBase64String(envelope.Nonce);
            tag = Convert.FromBase64String(envelope.Tag);
            cipher = Convert.FromBase64String(envelope.Data);
        }
        catch (FormatException ex)
        {
            throw new BackupFormatException("The encrypted backup is damaged.", ex);
        }

        if (nonce.Length != NonceBytes || tag.Length != TagBytes || salt.Length < 8)
        {
            throw new BackupFormatException("The encrypted backup is damaged.");
        }

        byte[] key = DeriveKey(passphrase, salt, envelope.Iterations);
        byte[] plain = new byte[cipher.Length];
        try
        {
            using AesGcm aes = new(key, TagBytes);
            aes.Decrypt(nonce, cipher, tag, plain, AssociatedData(envelope.Version));
            return Encoding.UTF8.GetString(plain);
        }
        catch (CryptographicException ex)
        {
            // GCM cannot tell a wrong passphrase from a changed file; both end here.
            throw new BackupPassphraseException("The passphrase is wrong or the file was changed.") { HResult = ex.HResult };
        }
        finally
        {
            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(plain);
        }
    }

    private static byte[] DeriveKey(string passphrase, byte[] salt, int iterations) =>
        Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(passphrase.Normalize(NormalizationForm.FormKC)), salt, iterations, HashAlgorithmName.SHA256, KeyBytes);

    private static byte[] AssociatedData(int version) => Encoding.ASCII.GetBytes($"{FormatName}/{version}");
}
