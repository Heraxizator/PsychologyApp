using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Analytics;
using PsychologyApp.Application.Services.Statistic;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.Analytics;

public sealed class PageAnalyticsServiceIntegrationTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private ServiceProvider _provider = null!;

    public Task InitializeAsync()
    {
        _provider = IntegrationTestServiceCollection.BuildCoreProvider(_connectionFactory);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task TrackPageVisitAsync_PersistsStatisticQueryableByService()
    {
        IPageAnalyticsService analytics = _provider.GetRequiredService<IPageAnalyticsService>();
        IStatisticService statistics = _provider.GetRequiredService<IStatisticService>();

        long before = await statistics.CountPageCompletedAsync();
        var startedAt = DateTime.UtcNow.AddSeconds(-15);

        await analytics.TrackPageVisitAsync("Integration", $"Page-{Guid.NewGuid():N}", startedAt);

        long after = await statistics.CountPageCompletedAsync();

        Assert.True(after > before);
    }

    public async Task DisposeAsync()
    {
        await _connectionFactory.DisposeAsync();
        await _provider.DisposeAsync();
    }
}
