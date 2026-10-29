namespace PsychologyApp.Application.Abstractions.Persistence;

/// <summary>Supplies the key the database file is encrypted with; the platform decides where the key lives (the Android Keystore, the iOS Keychain).</summary>
public interface IDatabaseKeyProvider
{
    /// <summary>
    /// A 64-character hexadecimal key (32 random bytes), created on first use. Null means the required key is unavailable:
    /// callers must refuse database access and may retry later, never fall back to plaintext.
    /// </summary>
    Task<string?> GetOrCreateKeyAsync(CancellationToken cancellationToken = default);
}
