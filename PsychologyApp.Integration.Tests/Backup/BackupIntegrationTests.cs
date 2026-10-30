using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.DataBackup;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Testing.Data;
using System.Text.Json;
using Xunit;

namespace PsychologyApp.Integration.Tests.Backup;

public sealed class BackupIntegrationTests : IAsyncLifetime
{
    private static readonly DateTime LongAgo = new(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc);

    private readonly SharedMemoryConnectionFactory _sourceFactory = new();
    private readonly SharedMemoryConnectionFactory _targetFactory = new();
    private ServiceProvider _source = null!;
    private ServiceProvider _target = null!;

    public Task InitializeAsync()
    {
        _source = IntegrationTestServiceCollection.BuildCoreProvider(_sourceFactory);
        _target = IntegrationTestServiceCollection.BuildCoreProvider(_targetFactory);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task RoundTrip_KeepsOriginalTimestampsAndChats()
    {
        await _source.GetRequiredService<IUserProgressService>().RecordMoodAsync(4, "calm", LongAgo);
        IChatRepository sourceChat = _source.GetRequiredService<IChatRepository>();
        long sessionId = await sourceChat.CreateSessionAsync("first", LongAgo);
        await sourceChat.AddMessageAsync(new ChatMessageDTO { SessionId = sessionId, Role = ChatRole.User, Text = "hi", CreatedAt = LongAgo });
        await sourceChat.SetMemoryAsync("name", "Sam");

        string json = await _source.GetRequiredService<IBackupService>().ExportAsync();
        BackupImportResult result = await _target.GetRequiredService<IBackupService>().ImportAsync(json);

        Assert.Equal(1, result.MoodEntries);
        Assert.Equal(1, result.ChatSessions);
        IReadOnlyList<MoodEntryDTO> moods = await _target.GetRequiredService<IUserProgressService>().GetMoodsAsync();
        Assert.Equal(LongAgo, Assert.Single(moods).RecordedAt);
        IChatRepository targetChat = _target.GetRequiredService<IChatRepository>();
        ChatSessionDTO session = Assert.Single(await targetChat.GetSessionsAsync());
        Assert.Equal("hi", Assert.Single(await targetChat.GetMessagesAsync(session.Id)).Text);
        Assert.Equal("Sam", (await targetChat.GetMemoryAsync())["name"]);
    }

    [Fact]
    public async Task EncryptedBackup_NeedsThePassphrase_AndRestoresTheSameData()
    {
        await _source.GetRequiredService<IUserProgressService>().RecordMoodAsync(4, "a private note", LongAgo);
        IBackupService source = _source.GetRequiredService<IBackupService>();
        IBackupService target = _target.GetRequiredService<IBackupService>();

        string file = await source.ExportEncryptedAsync("correct horse battery");

        Assert.True(target.IsEncrypted(file));
        Assert.DoesNotContain("a private note", file);
        await Assert.ThrowsAsync<BackupPassphraseException>(() => target.ImportAsync(file));
        await Assert.ThrowsAsync<BackupPassphraseException>(() => target.ImportEncryptedAsync(file, "wrong passphrase"));
        Assert.Empty(await _target.GetRequiredService<IUserProgressService>().GetMoodsAsync());

        BackupImportResult result = await target.ImportEncryptedAsync(file, "correct horse battery");

        Assert.Equal(1, result.MoodEntries);
        Assert.Equal("a private note", Assert.Single(await _target.GetRequiredService<IUserProgressService>().GetMoodsAsync()).Note);
    }

    [Fact]
    public async Task Import_SameFileTwice_AddsNothingTheSecondTime()
    {
        await _source.GetRequiredService<IUserProgressService>().RecordMoodAsync(2, null, LongAgo);
        await _source.GetRequiredService<IUserProgressService>().SaveTestResultAsync("gad7", 5, "mild");
        IBackupService target = _target.GetRequiredService<IBackupService>();
        string json = await _source.GetRequiredService<IBackupService>().ExportAsync();

        await target.ImportAsync(json);
        BackupImportResult second = await target.ImportAsync(json);

        Assert.Equal(0, second.MoodEntries);
        Assert.Equal(0, second.TestResults);
        Assert.True(second.SkippedDuplicates >= 2);
        Assert.Equal(1, await _target.GetRequiredService<IUserProgressService>().CountTestResultsAsync());
        Assert.Single(await _target.GetRequiredService<IUserProgressService>().GetMoodsAsync());
    }

    [Fact]
    public async Task Import_SessionResults_KeepTheirDateAndDoNotCreateCompletionsNow()
    {
        AppBackupDTO backup = new()
        {
            SessionResults = [new SessionResultDTO { ItemKey = "breathing", CompletedAt = LongAgo, DurationSeconds = 60, PreIntensity = 7, PostIntensity = 3, Note = "better" }]
        };
        IUserProgressService progress = _target.GetRequiredService<IUserProgressService>();

        await _target.GetRequiredService<IBackupService>().ImportAsync(JsonSerializer.Serialize(backup, BackupJsonContext.Default.AppBackupDTO));

        SessionResultDTO restored = Assert.Single(await progress.GetRecentSessionResultsAsync());
        Assert.Equal(LongAgo, restored.CompletedAt);
        Assert.Equal(3, restored.PostIntensity);
        Assert.Equal("better", restored.Note);
        Assert.Equal(0, await progress.CountTechniqueCompletionsAsync());
        Assert.Null(await progress.GetLastTechniqueCompletionDateAsync());
        Assert.Equal(0, await progress.GetStreakDaysAsync());
    }

    [Fact]
    public async Task Import_WhenARowFails_RollsEverythingBack()
    {
        string json = """
            {
              "FormatVersion": 2,
              "MoodEntries": [ { "MoodLevel": 3, "RecordedAt": "2020-01-02T03:04:05Z" } ],
              "ChatSessions": [ {
                "Session": { "Title": "t", "CreatedAt": "2020-01-02T03:04:05Z", "UpdatedAt": "2020-01-02T03:04:05Z" },
                "Messages": [ { "Role": 0, "Text": null, "CreatedAt": "2020-01-02T03:04:05Z" } ]
              } ]
            }
            """;

        await Assert.ThrowsAnyAsync<Exception>(() => _target.GetRequiredService<IBackupService>().ImportAsync(json));

        Assert.Empty(await _target.GetRequiredService<IUserProgressService>().GetMoodsAsync());
        Assert.Empty(await _target.GetRequiredService<IChatRepository>().GetSessionsAsync());
    }

    [Fact]
    public async Task Import_WhenDocumentWriteFails_RollsBackRows()
    {
        await using var connection = await _targetFactory.CreateOpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "CREATE TRIGGER FailSafetyPlan BEFORE INSERT ON AppMetadata WHEN NEW.Key = 'SafetyPlan' BEGIN SELECT RAISE(ABORT, 'injected failure'); END;";
        await command.ExecuteNonQueryAsync();
        AppBackupDTO backup = new()
        {
            MoodEntries = [new MoodEntryDTO { MoodLevel = 3, RecordedAt = LongAgo }],
            SafetyPlan = new SafetyPlanDTO { WarningSigns = ["warning"] }
        };
        await Assert.ThrowsAnyAsync<Exception>(() => _target.GetRequiredService<IBackupService>().ImportAsync(
            JsonSerializer.Serialize(backup, BackupJsonContext.Default.AppBackupDTO)));
        Assert.Empty(await _target.GetRequiredService<IUserProgressService>().GetMoodsAsync());
    }

    [Fact]
    public async Task Import_ContinuedChat_MergesMessagesAndKeepsNewestState()
    {
        IBackupService target = _target.GetRequiredService<IBackupService>();
        string Snapshot(int count) => JsonSerializer.Serialize(new AppBackupDTO
        {
            ChatSessions = [new BackupChatSessionDTO
            {
                Session = new ChatSessionDTO { Title = "chat", CreatedAt = LongAgo, UpdatedAt = LongAgo.AddMinutes(count), StateJson = count.ToString() },
                Messages = Enumerable.Range(1, count).Select(i => new ChatMessageDTO
                { Id = i, Role = ChatRole.User, Text = "repeated message", CreatedAt = LongAgo }).ToList()
            }]
        }, BackupJsonContext.Default.AppBackupDTO);
        await target.ImportAsync(Snapshot(1));
        await target.ImportAsync(Snapshot(2));
        await target.ImportAsync(Snapshot(2));
        await target.ImportAsync(Snapshot(1));
        IChatRepository chat = _target.GetRequiredService<IChatRepository>();
        ChatSessionDTO session = Assert.Single(await chat.GetSessionsAsync());
        Assert.Equal("2", session.StateJson);
        Assert.Equal(2, (await chat.GetMessagesAsync(session.Id)).Count);
    }

    [Fact]
    public async Task Import_ChatDiscardsUndefinedFeelingsAndOutOfRangeTension()
    {
        AppBackupDTO backup = new()
        {
            ChatSessions = [new BackupChatSessionDTO
            {
                Session = new ChatSessionDTO
                {
                    Title = "chat",
                    CreatedAt = LongAgo,
                    UpdatedAt = LongAgo,
                    Emotion = "999",
                    Theme = "999",
                    FirstIntensity = 99,
                    LastIntensity = -1
                },
                Messages = [new ChatMessageDTO { Role = ChatRole.User, Text = "привет", CreatedAt = LongAgo }]
            }]
        };

        await _target.GetRequiredService<IBackupService>().ImportAsync(
            JsonSerializer.Serialize(backup, BackupJsonContext.Default.AppBackupDTO));

        ChatSessionDTO session = Assert.Single(await _target.GetRequiredService<IChatRepository>().GetSessionsAsync());
        Assert.Null(session.Emotion);
        Assert.Null(session.Theme);
        Assert.Null(session.FirstIntensity);
        Assert.Null(session.LastIntensity);
    }

    [Theory]
    [InlineData("\"MoodEntries\": null")]
    [InlineData("\"ChatSessions\": [null]")]
    [InlineData("\"ChatSessions\": [{\"Session\":null}]")]
    [InlineData("\"SafetyPlan\": {\"WarningSigns\":[\"warning\"],\"Contacts\":[null]}")]
    [InlineData("\"SafetyPlan\": {\"Contacts\":[{\"Name\":null,\"Phone\":null}]}")]
    public async Task Import_NullRequiredData_IsRejectedBeforeAnyWrites(string property)
    {
        await _target.GetRequiredService<IUserProgressService>().RecordMoodAsync(4, "existing", LongAgo);
        string json = "{\"FormatVersion\":2," + property + "}";

        await Assert.ThrowsAsync<BackupFormatException>(() => _target.GetRequiredService<IBackupService>().ImportAsync(json));

        Assert.Equal("existing", Assert.Single(await _target.GetRequiredService<IUserProgressService>().GetMoodsAsync()).Note);
        Assert.Null(await _target.GetRequiredService<IClinicalCareRepository>().GetSafetyPlanAsync());
    }

    [Theory]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("{\"FormatVersion\": 99}")]
    [InlineData("{\"FormatVersion\": 0}")]
    public async Task Import_UnreadableOrUnsupportedFile_ThrowsBackupFormatException(string json)
    {
        await Assert.ThrowsAsync<BackupFormatException>(() => _target.GetRequiredService<IBackupService>().ImportAsync(json));
    }

    [Fact]
    public async Task Import_OlderRiskAssessment_NeverReplacesANewerOne()
    {
        IClinicalCareRepository clinical = _target.GetRequiredService<IClinicalCareRepository>();
        await clinical.SaveRiskAssessmentAsync(new RiskAssessmentDTO { AssessedAt = DateTime.UtcNow, Source = "manual", RiskLevel = PsychologyApp.Domain.ClinicalCare.RiskLevel.Green });
        AppBackupDTO backup = new()
        {
            LatestRiskAssessment = new RiskAssessmentDTO { AssessedAt = LongAgo, Source = "manual", RiskLevel = PsychologyApp.Domain.ClinicalCare.RiskLevel.Red }
        };

        await _target.GetRequiredService<IBackupService>().ImportAsync(JsonSerializer.Serialize(backup, BackupJsonContext.Default.AppBackupDTO));

        RiskAssessmentDTO? latest = await clinical.GetLatestRiskAssessmentAsync();
        Assert.Equal(PsychologyApp.Domain.ClinicalCare.RiskLevel.Green, latest?.RiskLevel);
    }

    public async Task DisposeAsync()
    {
        await _sourceFactory.DisposeAsync();
        await _targetFactory.DisposeAsync();
        await _source.DisposeAsync();
        await _target.DisposeAsync();
    }
}
