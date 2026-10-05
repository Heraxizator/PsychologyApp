using System.Data.Common;
using Microsoft.Data.Sqlite;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Infrastructure.Data.Context;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.Data;

/// <summary>A file database in WAL mode, like the app's: readers may overlap, writers may not.</summary>
public sealed class SqliteOperationLaneReadersTests : IAsyncLifetime
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"lane-{Guid.NewGuid():N}.db");
    private FileFactory _factory = null!;

    private sealed class FileFactory(string path) : IDbConnectionFactory
    {
        public string DatabasePath => path;

        public bool AllowsParallelReads => true;

        public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            SqliteConnection connection = new($"Data Source={path};Pooling=False");
            await connection.OpenAsync(cancellationToken);
            await SqliteSchema.ConfigureConnectionAsync(connection, cancellationToken);
            return connection;
        }
    }

    public Task InitializeAsync()
    {
        _factory = new FileFactory(_path);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        SqliteConnection.ClearAllPools();
        foreach (string file in new[] { _path, _path + "-wal", _path + "-shm" })
        {
            try
            {
                File.Delete(file);
            }
            catch (IOException)
            {
            }
        }

        return Task.CompletedTask;
    }

    private static async Task<SqliteConnection> Open(IDbConnectionFactory f, bool read) =>
        await SqliteOperationLane.OpenAsync(f, CancellationToken.None, read);

    [Fact]
    public async Task Two_readers_are_open_at_the_same_time()
    {
        await using SqliteConnection first = await Open(_factory, read: true);
        Task<SqliteConnection> second = Open(_factory, read: true);

        await using SqliteConnection opened = await second.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(System.Data.ConnectionState.Open, opened.State);
    }

    [Fact]
    public async Task A_reader_waits_for_a_writer_requested_before_it()
    {
        SqliteConnection writer = await Open(_factory, read: false);
        Task<SqliteConnection> reader = Open(_factory, read: true);

        await Task.Delay(200);
        Assert.False(reader.IsCompleted);

        await writer.DisposeAsync();
        await using SqliteConnection opened = await reader.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task A_writer_waits_for_every_reader_requested_before_it()
    {
        SqliteConnection reader1 = await Open(_factory, read: true);
        SqliteConnection reader2 = await Open(_factory, read: true);
        Task<SqliteConnection> writer = Open(_factory, read: false);

        await reader1.DisposeAsync();
        await Task.Delay(200);
        Assert.False(writer.IsCompleted);

        await reader2.DisposeAsync();
        await using SqliteConnection opened = await writer.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task A_reader_requested_after_a_writer_does_not_overtake_it_even_with_readers_before()
    {
        SqliteConnection reader = await Open(_factory, read: true);
        Task<SqliteConnection> writer = Open(_factory, read: false);
        Task<SqliteConnection> laterReader = Open(_factory, read: true);

        await Task.Delay(200);
        Assert.False(writer.IsCompleted);
        Assert.False(laterReader.IsCompleted);

        await reader.DisposeAsync();
        await using SqliteConnection w = await writer.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.False(laterReader.IsCompleted);

        await w.DisposeAsync();
        await using SqliteConnection r = await laterReader.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task A_database_that_does_not_allow_parallel_reads_keeps_one_operation_at_a_time()
    {
        await using PsychologyApp.Testing.Data.SharedMemoryConnectionFactory memory = new();
        SqliteConnection first = await Open(memory, read: true);
        Task<SqliteConnection> second = Open(memory, read: true);

        await Task.Delay(200);
        Assert.False(second.IsCompleted);

        await first.DisposeAsync();
        await using SqliteConnection opened = await second.WaitAsync(TimeSpan.FromSeconds(5));
    }
}
