using System.Data.Common;
using Dapper;
using PsychologyApp.Application.Body;
using PsychologyApp.Infrastructure.Data.Repositories.UserProgress;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.Data;

/// <summary>The small JSON documents in AppMetadata (fear ladder, reading, body log) through the shared base.</summary>
public sealed class AppMetadataDocumentRepositoryTests
{
    private readonly SharedMemoryConnectionFactory _factory = new();

    [Fact]
    public async Task A_saved_document_reads_back()
    {
        BodyRepository repository = new(_factory, RepositoryTestContext.Settings);
        BodyEntry entry = new("a", new DateTime(2026, 10, 17, 8, 0, 0, DateTimeKind.Utc), BodyArea.Throat, BodySensation.Lump, null, [BodyFeeling.Hurt], "note");

        await repository.SaveAsync(new BodyLog { Entries = [entry] });

        BodyEntry read = Assert.Single((await repository.GetAsync())!.Entries);
        Assert.Equal((entry.Id, entry.Area, entry.Intensity, entry.Note), (read.Id, read.Area, read.Intensity, read.Note));
        Assert.Equal([BodyFeeling.Hurt], read.Feelings);
    }

    [Fact]
    public async Task An_unreadable_document_is_set_aside_before_the_next_save_replaces_it()
    {
        BodyRepository repository = new(_factory, RepositoryTestContext.Settings);
        await PutAsync(BodyRepository.MetadataKey, "{ not json");

        Assert.Null(await repository.GetAsync());
        await repository.SaveAsync(new BodyLog());

        Assert.Equal("{ not json", await GetAsync(repository.UnreadableKey));
    }

    [Fact]
    public async Task A_place_from_a_newer_version_keeps_the_rest_of_the_log_readable()
    {
        BodyRepository repository = new(_factory, RepositoryTestContext.Settings);
        await PutAsync(BodyRepository.MetadataKey, """
            {"Entries":[
              {"Id":"new","CreatedUtc":"2026-10-17T08:00:00Z","Area":"Knees","Sensation":"Pain","Intensity":3,"Feelings":["Awe","Fear"],"Note":null},
              {"Id":"old","CreatedUtc":"2026-10-16T08:00:00Z","Area":"Chest","Sensation":"Tight","Intensity":null,"Feelings":["Anxiety"],"Note":null}
            ]}
            """);

        BodyLog log = (await repository.GetAsync())!;

        Assert.Equal(2, log.Entries.Count);
        Assert.Equal(["old"], log.Readable().Select(entry => entry.Id));
        Assert.Null(await GetAsync(repository.UnreadableKey));
    }

    private async Task PutAsync(string key, string value)
    {
        await using DbConnection connection = await _factory.CreateOpenConnectionAsync();
        await connection.ExecuteAsync(
            "INSERT INTO AppMetadata (Key, Value) VALUES (@key, @value) ON CONFLICT(Key) DO UPDATE SET Value = excluded.Value;",
            new { key, value });
    }

    private async Task<string?> GetAsync(string key)
    {
        await using DbConnection connection = await _factory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<string?>("SELECT Value FROM AppMetadata WHERE Key = @key;", new { key });
    }
}
