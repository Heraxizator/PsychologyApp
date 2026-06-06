using PsychologyApp.Domain.Entities;
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

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _connectionFactory.DisposeAsync();
}
