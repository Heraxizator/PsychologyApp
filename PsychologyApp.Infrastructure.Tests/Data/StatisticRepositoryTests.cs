using PsychologyApp.Domain.Entities;
using PsychologyApp.Testing.Data;
using PsychologyApp.Infrastructure.Data.Repositories.Statistics;
using Xunit;

namespace PsychologyApp.Infrastructure.Tests.Data;

public class StatisticRepositoryTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private readonly StatisticRepository _repository;

    public StatisticRepositoryTests()
    {
        _repository = new StatisticRepository(_connectionFactory, RepositoryTestContext.Settings);
    }

    [Fact]
    public async Task AddAsync_RoundTripsDateTime()
    {
        var expected = new DateTime(2026, 6, 5, 14, 30, 0, DateTimeKind.Utc);
        var statistic = Statistic.Create("Module", "Page", expected, 42);

        long id = await _repository.AddAsync(statistic);
        Statistic? loaded = await _repository.GetByIdAsync(id);

        Assert.NotNull(loaded);
        Assert.Equal(expected.ToUniversalTime(), loaded!.DateTime.ToUniversalTime());
        Assert.Equal(42, loaded.SecondsDuration);
    }

    [Fact]
    public async Task CountByPageNameAsync_CountsMatchingEntries()
    {
        string pageName = $"Page-{Guid.NewGuid():N}";
        await _repository.AddAsync(Statistic.Create("M1", pageName, DateTime.UtcNow, 1));
        await _repository.AddAsync(Statistic.Create("M1", pageName, DateTime.UtcNow, 2));

        long count = await _repository.CountByPageNameAsync(pageName);

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task PruneAsync_DeletesOldRowsButKeepsTheLatestRowOfEveryPage()
    {
        DateTime now = new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
        await _repository.AddAsync(Statistic.Create("M", "Old only", now.AddDays(-200), 1));
        await _repository.AddAsync(Statistic.Create("M", "Both", now.AddDays(-200), 1));
        await _repository.AddAsync(Statistic.Create("M", "Both", now.AddDays(-100), 1));
        await _repository.AddAsync(Statistic.Create("M", "Both", now.AddDays(-1), 1));
        await _repository.AddAsync(Statistic.Create("M", "Recent", now.AddDays(-2), 1));

        int removed = await _repository.PruneAsync(now.AddDays(-90));

        Assert.Equal(2, removed);
        Assert.Equal(3, await _repository.CountDistinctPagesAsync());
        Assert.Equal(1, await _repository.CountByPageNameAsync("Old only"));
        Assert.Equal(1, await _repository.CountByPageNameAsync("Both"));
    }

    [Fact]
    public async Task PruneAsync_OnAnEmptyTable_RemovesNothing() =>
        Assert.Equal(0, await _repository.PruneAsync(DateTime.UtcNow));

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _connectionFactory.DisposeAsync();}
