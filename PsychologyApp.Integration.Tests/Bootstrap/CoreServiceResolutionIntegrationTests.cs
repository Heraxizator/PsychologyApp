using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Abstractions.Startup;
using PsychologyApp.Application.Quot;
using PsychologyApp.Application.Statistic;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.Bootstrap;

public sealed class CoreServiceResolutionIntegrationTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private ServiceProvider _provider = null!;

    public Task InitializeAsync()
    {
        _provider = IntegrationTestServiceCollection.BuildCoreProvider(_connectionFactory, services =>
        {
            IntegrationTestServiceCollection.ReplaceSingleton<IQuotContentProvider>(
                services,
                new CachedQuotContentProvider(new StubQuotContentProvider()));
        });
        return Task.CompletedTask;
    }

    [Fact]
    public void AddPsychologyAppCore_ResolvesCoreServices()
    {
        Assert.NotNull(_provider.GetRequiredService<IDatabaseInitializer>());
        Assert.NotNull(_provider.GetRequiredService<IQuotService>());
        Assert.NotNull(_provider.GetRequiredService<IUserProgressService>());
        Assert.NotNull(_provider.GetRequiredService<IStatisticService>());
        Assert.NotNull(_provider.GetRequiredService<IAppStartupService>());
    }

    public async Task DisposeAsync()
    {
        await _connectionFactory.DisposeAsync();
        await _provider.DisposeAsync();
    }

    private sealed class StubQuotContentProvider : IQuotContentProvider
    {
        public Task<IReadOnlyList<QuotSeed>> LoadAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<QuotSeed>>(
            [
                new QuotSeed("Title", "Text", "Theme")
            ]);
    }
}
