namespace PsychologyApp.Application.Abstractions.Persistence;

/// <summary>Supplies the key the database file is encrypted with; the platform decides where the key lives (the Android Keystore, the iOS Keychain).</summary>
public interface IDatabaseKeyProvider
{
    /// <summary>Reads the existing 64-character hexadecimal key without creating a replacement. Null means no key is stored.</summary>
    Task<string?> GetKeyAsync(CancellationToken cancellationToken = default);

    /// <summary>Creates and stores a key only after the caller has confirmed that replacing a missing key is safe.</summary>
    Task<string?> CreateKeyAsync(CancellationToken cancellationToken = default);
}
