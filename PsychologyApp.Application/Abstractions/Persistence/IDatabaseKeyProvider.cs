namespace PsychologyApp.Application.Abstractions.Persistence;

/// <summary>Supplies the key the database file is encrypted with; the platform decides where the key lives (the Android Keystore, the iOS Keychain).</summary>
public interface IDatabaseKeyProvider
{
    /// <summary>
    /// A 64-character hexadecimal key (32 random bytes), created on first use. Null means this platform cannot keep a key
    /// and the database stays unencrypted.
    /// </summary>
    Task<string?> GetOrCreateKeyAsync(CancellationToken cancellationToken = default);
}
