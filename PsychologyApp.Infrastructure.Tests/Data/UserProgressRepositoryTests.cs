using PsychologyApp.Application.Models;
using PsychologyApp.Infrastructure.Data.Repositories.UserProgress;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.Data;

public sealed class UserProgressRepositoryTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private readonly UserProgressRepository _repository;

    public UserProgressRepositoryTests()
    {
        _repository = new UserProgressRepository(_connectionFactory, RepositoryTestContext.Settings);
    }

    [Fact]
    public async Task SaveTestResultAsync_GetLatestTestResultAsync_ReturnsMostRecent()
    {
        await _repository.SaveTestResultAsync(new TestResultDTO
        {
            TestId = "beck",
            Score = 5,
            Summary = "low",
            CompletedAt = DateTime.UtcNow.AddDays(-1)
        });

        await _repository.SaveTestResultAsync(new TestResultDTO
        {
            TestId = "beck",
            Score = 12,
            Summary = "high",
            CompletedAt = DateTime.UtcNow
        });

        TestResultDTO? latest = await _repository.GetLatestTestResultAsync("beck");

        Assert.NotNull(latest);
        Assert.Equal(12, latest!.Score);
        Assert.Equal("high", latest.Summary);
    }

    [Fact]
    public async Task GetTestResultHistoryAsync_ReturnsNewestFirst()
    {
        await _repository.SaveTestResultAsync(new TestResultDTO { TestId = "beck", Score = 1, Summary = "a", CompletedAt = DateTime.UtcNow.AddHours(-2) });
        await _repository.SaveTestResultAsync(new TestResultDTO { TestId = "beck", Score = 2, Summary = "b", CompletedAt = DateTime.UtcNow.AddHours(-1) });
        await _repository.SaveTestResultAsync(new TestResultDTO { TestId = "beck", Score = 3, Summary = "c", CompletedAt = DateTime.UtcNow });

        IReadOnlyList<TestResultDTO> history = await _repository.GetTestResultHistoryAsync("beck", 2);

        Assert.Equal(2, history.Count);
        Assert.Equal(3, history[0].Score);
        Assert.Equal(2, history[1].Score);
    }

    [Fact]
    public async Task CountTestResultsAsync_CountsAllRows()
    {
        await _repository.SaveTestResultAsync(new TestResultDTO { TestId = "a", Score = 1, Summary = "s", CompletedAt = DateTime.UtcNow });
        await _repository.SaveTestResultAsync(new TestResultDTO { TestId = "b", Score = 2, Summary = "s", CompletedAt = DateTime.UtcNow });

        long count = await _repository.CountTestResultsAsync();

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task RecordCompletionAsync_CountTechniqueCompletionsAsync_TracksTechniques()
    {
        await _repository.RecordCompletionAsync(new CompletionDTO
        {
            CompletionKind = "technique",
            ItemKey = "Spin",
            ModuleName = "Practice",
            PageName = "Spin",
            CompletedAt = DateTime.UtcNow,
            DurationSeconds = 30
        });

        long count = await _repository.CountTechniqueCompletionsAsync();

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetLastTechniqueCompletionDateAsync_ReturnsLatestTechniqueCompletion()
    {
        DateTime older = DateTime.UtcNow.AddDays(-2);
        DateTime newer = DateTime.UtcNow.AddDays(-1);

        await _repository.RecordCompletionAsync(new CompletionDTO
        {
            CompletionKind = "mood",
            ItemKey = "mood",
            ModuleName = "Practice",
            PageName = "Mood",
            CompletedAt = older,
            DurationSeconds = 0
        });

        await _repository.RecordCompletionAsync(new CompletionDTO
        {
            CompletionKind = "technique",
            ItemKey = "Paper",
            ModuleName = "Practice",
            PageName = "Paper",
            CompletedAt = newer,
            DurationSeconds = 10
        });

        DateTime? last = await _repository.GetLastTechniqueCompletionDateAsync();

        Assert.NotNull(last);
        Assert.True(Math.Abs((newer - last!.Value).TotalSeconds) < 1);
    }

    [Fact]
    public async Task GetCompletionDatesAsync_ReturnsDistinctDaysDescending()
    {
        DateTime day1 = DateTime.UtcNow.Date;
        DateTime day2 = day1.AddDays(-1);

        await _repository.RecordCompletionAsync(new CompletionDTO
        {
            CompletionKind = "technique",
            ItemKey = "a",
            ModuleName = "Practice",
            PageName = "A",
            CompletedAt = day1.AddHours(1),
            DurationSeconds = 0
        });

        await _repository.RecordCompletionAsync(new CompletionDTO
        {
            CompletionKind = "technique",
            ItemKey = "b",
            ModuleName = "Practice",
            PageName = "B",
            CompletedAt = day1.AddHours(2),
            DurationSeconds = 0
        });

        await _repository.RecordCompletionAsync(new CompletionDTO
        {
            CompletionKind = "technique",
            ItemKey = "c",
            ModuleName = "Practice",
            PageName = "C",
            CompletedAt = day2.AddHours(3),
            DurationSeconds = 0
        });

        IReadOnlyList<DateOnly> dates = await _repository.GetCompletionDatesAsync();

        Assert.Equal(2, dates.Count);
        Assert.Equal(DateOnly.FromDateTime(day1), dates[0]);
        Assert.Equal(DateOnly.FromDateTime(day2), dates[1]);
    }

    [Fact]
    public async Task GetLastCompletionForItemAsync_ReturnsLatestForItemKey()
    {
        DateTime older = DateTime.UtcNow.AddDays(-3);
        DateTime newer = DateTime.UtcNow.AddDays(-1);

        await _repository.RecordCompletionAsync(new CompletionDTO
        {
            CompletionKind = "technique",
            ItemKey = "Spin",
            ModuleName = "Practice",
            PageName = "Spin",
            CompletedAt = older,
            DurationSeconds = 0
        });

        await _repository.RecordCompletionAsync(new CompletionDTO
        {
            CompletionKind = "technique",
            ItemKey = "Spin",
            ModuleName = "Practice",
            PageName = "Spin",
            CompletedAt = newer,
            DurationSeconds = 0
        });

        DateTime? last = await _repository.GetLastCompletionForItemAsync("Spin");

        Assert.NotNull(last);
        Assert.True(Math.Abs((newer - last!.Value).TotalSeconds) < 1);
    }

    [Fact]
    public async Task SessionDraft_SaveGetDelete_Works()
    {
        const string key = "Paper";
        const string payload = """{"fields":{"0":"draft"}}""";

        await _repository.SaveSessionDraftAsync(key, payload);
        string? loaded = await _repository.GetSessionDraftAsync(key);

        Assert.Equal(payload, loaded);

        await _repository.DeleteSessionDraftAsync(key);
        string? afterDelete = await _repository.GetSessionDraftAsync(key);

        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task SaveSessionDraftAsync_UpsertsExistingDraft()
    {
        const string key = "Check";

        await _repository.SaveSessionDraftAsync(key, "v1");
        await _repository.SaveSessionDraftAsync(key, "v2");

        string? loaded = await _repository.GetSessionDraftAsync(key);

        Assert.Equal("v2", loaded);
    }

    [Fact]
    public async Task RecordMoodAsync_PersistsMoodAndCompletion()
    {
        DateTime recordedAt = DateTime.UtcNow;
        await _repository.RecordMoodAsync(new MoodEntryDTO
        {
            MoodLevel = 4,
            Note = "ok",
            RecordedAt = recordedAt
        });

        IReadOnlyList<MoodEntryDTO> moods = await _repository.GetRecentMoodsAsync(1);
        IReadOnlyList<DateOnly> completionDates = await _repository.GetCompletionDatesAsync();

        Assert.Single(moods);
        Assert.Equal(4, moods[0].MoodLevel);
        Assert.Equal("ok", moods[0].Note);
        Assert.Contains(DateOnly.FromDateTime(recordedAt), completionDates);
    }

    [Fact]
    public async Task GetRecentMoodsAsync_ReturnsNewestFirst()
    {
        await _repository.RecordMoodAsync(new MoodEntryDTO { MoodLevel = 1, RecordedAt = DateTime.UtcNow.AddHours(-2) });
        await _repository.RecordMoodAsync(new MoodEntryDTO { MoodLevel = 5, RecordedAt = DateTime.UtcNow });

        IReadOnlyList<MoodEntryDTO> moods = await _repository.GetRecentMoodsAsync(1);

        Assert.Single(moods);
        Assert.Equal(5, moods[0].MoodLevel);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _connectionFactory.DisposeAsync();
}
