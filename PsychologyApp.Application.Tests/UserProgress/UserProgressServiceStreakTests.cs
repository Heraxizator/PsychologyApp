using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using Xunit;

namespace PsychologyApp.Application.Tests.UserProgress;

public sealed class UserProgressServiceStreakTests
{
    [Fact]
    public async Task GetStreakDaysAsync_WhenNoCompletions_ReturnsZero()
    {
        var repository = new FakeUserProgressRepository();
        var service = new UserProgressService(repository);

        int streak = await service.GetStreakDaysAsync();

        Assert.Equal(0, streak);
    }

    [Fact]
    public async Task GetStreakDaysAsync_WhenTodayCompleted_ReturnsOne()
    {
        var repository = new FakeUserProgressRepository
        {
            CompletionDates = [DateOnly.FromDateTime(DateTime.Today)]
        };
        var service = new UserProgressService(repository);

        int streak = await service.GetStreakDaysAsync();

        Assert.Equal(1, streak);
    }

    [Fact]
    public async Task GetStreakDaysAsync_CountsConsecutiveDaysFromToday()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        var repository = new FakeUserProgressRepository
        {
            CompletionDates = [today, today.AddDays(-1), today.AddDays(-2)]
        };
        var service = new UserProgressService(repository);

        int streak = await service.GetStreakDaysAsync();

        Assert.Equal(3, streak);
    }

    [Fact]
    public async Task GetStreakDaysAsync_StopsAtFirstGap()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        var repository = new FakeUserProgressRepository
        {
            CompletionDates = [today, today.AddDays(-2)]
        };
        var service = new UserProgressService(repository);

        int streak = await service.GetStreakDaysAsync();

        Assert.Equal(1, streak);
    }

    private sealed class FakeUserProgressRepository : IUserProgressRepository
    {
        public IReadOnlyList<DateOnly> CompletionDates { get; init; } = [];

        public Task<IReadOnlyList<DateOnly>> GetCompletionDatesAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(CompletionDates);

        public Task SaveTestResultAsync(TestResultDTO result, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default) => Task.FromResult<TestResultDTO?>(null);
        public Task<IReadOnlyList<TestResultDTO>> GetTestResultHistoryAsync(string testId, int limit, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<TestResultDTO>>([]);
        public Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default) => Task.FromResult(0L);
        public Task<DateTime?> GetLastTechniqueCompletionDateAsync(CancellationToken cancellationToken = default) => Task.FromResult<DateTime?>(null);
        public Task RecordCompletionAsync(CompletionDTO completion, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<long> CountTechniqueCompletionsAsync(CancellationToken cancellationToken = default) => Task.FromResult(0L);
        public Task<IReadOnlyList<CompletionDTO>> GetRecentTechniqueCompletionsAsync(int limit, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<CompletionDTO>>([]);
        public Task<DateTime?> GetLastCompletionForItemAsync(string itemKey, CancellationToken cancellationToken = default) => Task.FromResult<DateTime?>(null);
        public Task SaveSessionDraftAsync(string techniqueKey, string payloadJson, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<string?> GetSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);
        public Task DeleteSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RecordMoodAsync(MoodEntryDTO entry, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IReadOnlyList<MoodEntryDTO>> GetRecentMoodsAsync(int limit, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<MoodEntryDTO>>([]);
    }
}
