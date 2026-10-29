using Microsoft.Data.Sqlite;
using PsychologyApp.Application.Abstractions.Persistence;
using System.Data.Common;

namespace PsychologyApp.Infrastructure.Data.Context;

public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly IDatabaseKeyProvider? _keyProvider;
    private readonly SemaphoreSlim _readyGate = new(1, 1);
    private string? _key;
    private bool _ready;

    public SqliteConnectionFactory(IDatabaseKeyProvider? keyProvider = null)
    {
        _keyProvider = keyProvider;
        SqliteProvider.EnsureInitialized();
    }

    public string DatabasePath { get; } = SqlitePaths.GetDatabasePath();

    public bool AllowsParallelReads => true;

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        string? key = await GetReadyKeyAsync(cancellationToken).ConfigureAwait(false);

        // No Cache=Shared: shared-cache mode swaps WAL's reader/writer concurrency for table-level locks, and Microsoft
        // advises against combining it with WAL. Pooling already makes opening a fresh connection per operation cheap.
        var connection = new SqliteConnection($"Data Source={DatabasePath}");
        try
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            if (key is not null)
            {
                await SqliteEncryption.ApplyKeyAsync(connection, key, cancellationToken).ConfigureAwait(false);
            }

            await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken).ConfigureAwait(false);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    /// <summary>Before the first connection: fetches the key and moves an unencrypted database from an older build over to it.</summary>
    private async Task<string?> GetReadyKeyAsync(CancellationToken cancellationToken)
    {
        if (_ready)
        {
            return _key;
        }

        await _readyGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_ready)
            {
                return _key;
            }

            string? key = _keyProvider is null ? null : await _keyProvider.GetOrCreateKeyAsync(cancellationToken).ConfigureAwait(false);
            if (_keyProvider is not null && (key is null || !SqliteEncryption.IsValidKey(key)))
            {
                throw new InvalidOperationException("The required database encryption key is unavailable.");
            }
            if (key is not null && !await SqliteEncryption.EnsureEncryptedAsync(DatabasePath, key, cancellationToken).ConfigureAwait(false))
            {
                throw new InvalidOperationException("Database encryption could not be verified; access was refused.");
            }

            _key = key;
            _ready = true;
            return key;
        }
        finally
        {
            _readyGate.Release();
        }
    }
}
