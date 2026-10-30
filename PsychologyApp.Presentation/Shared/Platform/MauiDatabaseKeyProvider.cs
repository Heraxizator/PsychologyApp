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

    public Task<string?> GetKeyAsync(CancellationToken cancellationToken = default) => GetKeyCoreAsync(createIfMissing: false, cancellationToken);

    public Task<string?> CreateKeyAsync(CancellationToken cancellationToken = default) => GetKeyCoreAsync(createIfMissing: true, cancellationToken);

    private async Task<string?> GetKeyCoreAsync(bool createIfMissing, CancellationToken cancellationToken)
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
            if (stored is not null)
            {
                if (stored.Length == 64 && stored.All(Uri.IsHexDigit))
                {
                    return _cached = stored;
                }

                throw new InvalidOperationException("The stored database key is invalid; access was refused.");
            }

            if (!createIfMissing)
            {
                return null;
            }

            string created = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
            await SecureStorage.Default.SetAsync(StorageKey, created).ConfigureAwait(false);
            return _cached = created;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "The database key could not be read or stored; database access was refused");
            throw;
        }
        finally
        {
            _gate.Release();
        }
    }
}
