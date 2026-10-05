using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Data.Sqlite;

namespace PsychologyApp.Infrastructure.Data.Context;

/// <summary>
/// Encrypts the database file at rest (SQLite3 Multiple Ciphers, ChaCha20-Poly1305) and moves a database written by an older,
/// unencrypted build over to it. The original is kept until the encrypted file has been opened again and every table has the same
/// number of rows as before; if that check fails the original is put back.
/// </summary>
public static partial class SqliteEncryption
{
    private const string PlainCopySuffix = ".plain.bak";

    public static bool IsValidKey(string key) => KeyPattern().IsMatch(key);

    [GeneratedRegex("^[0-9a-fA-F]{64}$")]
    private static partial Regex KeyPattern();

    /// <summary>Applies the key to an open connection; must be the first statement on it.</summary>
    public static async Task ApplyKeyAsync(SqliteConnection connection, string key, CancellationToken cancellationToken = default)
    {
        if (!IsValidKey(key))
        {
            throw new ArgumentException("The database key must be 64 hexadecimal characters.", nameof(key));
        }

        await connection.ExecuteAsync(new CommandDefinition($"PRAGMA hexkey = '{key}';", cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    /// <summary>
    /// Makes the file at <paramref name="path"/> encrypted with <paramref name="key"/>. Returns false (and leaves the
    /// original, readable file in place) when the move could not be verified, so the app keeps working and tries again next start.
    /// </summary>
    public static async Task<bool> EnsureEncryptedAsync(string path, string key, CancellationToken cancellationToken = default)
    {
        SqliteProvider.EnsureInitialized();
        if (!File.Exists(path) || new FileInfo(path).Length == 0)
        {
            return true;
        }

        string plainCopy = path + PlainCopySuffix;
        if (!await IsPlainAsync(path, cancellationToken).ConfigureAwait(false))
        {
            // Already encrypted. A copy left behind by an interrupted move is removed once the key opens the file.
            if (File.Exists(plainCopy) && await CanOpenWithKeyAsync(path, key, cancellationToken).ConfigureAwait(false))
            {
                File.Delete(plainCopy);
            }

            return true;
        }

        File.Copy(path, plainCopy, overwrite: true);
        try
        {
            IReadOnlyDictionary<string, long> before = await CountRowsAsync(path, null, cancellationToken).ConfigureAwait(false);
            await RekeyAsync(path, key, cancellationToken).ConfigureAwait(false);
            IReadOnlyDictionary<string, long> after = await CountRowsAsync(path, key, cancellationToken).ConfigureAwait(false);

            if (before.Count != after.Count || before.Any(kv => !after.TryGetValue(kv.Key, out long n) || n != kv.Value))
            {
                throw new InvalidOperationException("The encrypted database does not have the same rows as the original.");
            }

            File.Delete(plainCopy);
            return true;
        }
        catch (Exception) when (RestorePlainCopy(path, plainCopy))
        {
            return false;
        }
    }

    private static bool RestorePlainCopy(string path, string plainCopy)
    {
        SqliteConnection.ClearAllPools();
        File.Copy(plainCopy, path, overwrite: true);
        TryDelete(path + "-wal");
        TryDelete(path + "-shm");
        File.Delete(plainCopy);
        return true;
    }

    private static async Task RekeyAsync(string path, string key, CancellationToken cancellationToken)
    {
        await using (SqliteConnection connection = Open(path))
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            // The key cannot be changed while the file is in WAL mode, and this also folds the WAL into the main file.
            await connection.ExecuteAsync(new CommandDefinition("PRAGMA journal_mode=DELETE;", cancellationToken: cancellationToken)).ConfigureAwait(false);
            await connection.ExecuteAsync(new CommandDefinition($"PRAGMA hexrekey = '{key}';", cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        SqliteConnection.ClearAllPools();
        TryDelete(path + "-wal");
        TryDelete(path + "-shm");
    }

    private static async Task<bool> IsPlainAsync(string path, CancellationToken cancellationToken)
    {
        try
        {
            await CountRowsAsync(path, null, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (SqliteException)
        {
            return false;
        }
    }

    private static async Task<bool> CanOpenWithKeyAsync(string path, string key, CancellationToken cancellationToken)
    {
        try
        {
            await CountRowsAsync(path, key, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (SqliteException)
        {
            return false;
        }
    }

    private static async Task<IReadOnlyDictionary<string, long>> CountRowsAsync(string path, string? key, CancellationToken cancellationToken)
    {
        await using SqliteConnection connection = Open(path);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        if (key is not null)
        {
            await ApplyKeyAsync(connection, key, cancellationToken).ConfigureAwait(false);
        }

        IEnumerable<string> tables = await connection.QueryAsync<string>(new CommandDefinition(
            "SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%' ORDER BY name;",
            cancellationToken: cancellationToken)).ConfigureAwait(false);

        Dictionary<string, long> counts = [];
        foreach (string table in tables)
        {
            counts[table] = await connection.ExecuteScalarAsync<long>(new CommandDefinition(
                $"SELECT COUNT(*) FROM \"{table.Replace("\"", "\"\"")}\";",
                cancellationToken: cancellationToken)).ConfigureAwait(false);
        }

        return counts;
    }

    private static SqliteConnection Open(string path) => new($"Data Source={path};Pooling=False");

    private static void TryDelete(string file)
    {
        try
        {
            File.Delete(file);
        }
        catch (IOException)
        {
        }
    }
}
