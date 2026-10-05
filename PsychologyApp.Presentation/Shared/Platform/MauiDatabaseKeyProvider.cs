using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using PsychologyApp.Application.Abstractions.Persistence;

namespace PsychologyApp.Presentation.Shared.Platform;

/// <summary>
/// Keeps the database key in SecureStorage (the Android Keystore, the iOS Keychain). The key is 32 random bytes made on first launch;
/// it is never shown, never part of a backup export, and goes away with the app's data when the app is uninstalled.
/// </summary>
public sealed class MauiDatabaseKeyProvider(ILogger<MauiDatabaseKeyProvider> logger) : IDatabaseKeyProvider
{
    private const string StorageKey = "database.key.v1";

    private readonly SemaphoreSlim _gate = new(1, 1);
    private string? _cached;

    public async Task<string?> GetOrCreateKeyAsync(CancellationToken cancellationToken = default)
    {
        if (_cached is not null)
        {
            return _cached;
        }

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_cached is not null)
            {
                return _cached;
            }

            string? stored = await SecureStorage.Default.GetAsync(StorageKey).ConfigureAwait(false);
            if (stored is { Length: 64 } && stored.All(Uri.IsHexDigit))
            {
                return _cached = stored;
            }

            string created = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
            await SecureStorage.Default.SetAsync(StorageKey, created).ConfigureAwait(false);
            return _cached = created;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // No secure storage on this device: the database stays as it was (unencrypted) instead of the app refusing to start.
            logger.LogError(ex, "The database key could not be read or stored; the database is not encrypted");
            return null;
        }
        finally
        {
            _gate.Release();
        }
    }
}
