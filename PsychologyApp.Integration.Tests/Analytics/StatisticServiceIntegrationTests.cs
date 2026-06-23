using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Statistic;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.Analytics;

public sealed class StatisticServiceIntegrationTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private ServiceProvider _provider = null!;

    public Task InitializeAsync()
    {
        _provider = IntegrationTestServiceCollection.BuildCoreProvider(_connectionFactory);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AddSingleAsync_PersistsStatisticThroughRepository()
    {
        IDatabaseInitializer initializer = _provider.GetRequiredService<IDatabaseInitializer>();
        IStatisticService statisticService = _provider.GetRequiredService<IStatisticService>();

        await initializer.InitializeAsync();

        await statisticService.AddSingleAsync(new StatisticDTO
        {
            ModuleName = "Practice",
            PageName = "Techniques",
            DateTime = DateTime.UtcNow,
            SecondsDuration = 42
        });

        long count = await statisticService.CountPageCompletedAsync();

        Assert.Equal(1, count);
    }

    public async Task DisposeAsync()
    {
        await _connectionFactory.DisposeAsync();
        await _provider.DisposeAsync();
    }
}
