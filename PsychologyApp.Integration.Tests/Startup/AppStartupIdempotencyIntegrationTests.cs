using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Abstractions.Startup;
using PsychologyApp.Application.Quot;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.Startup;

public sealed class AppStartupIdempotencyIntegrationTests : IAsyncLifetime
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
    public async Task InitializeAsync_CalledTwice_SucceedsAndKeepsSchema()
    {
        IAppStartupService startup = _provider.GetRequiredService<IAppStartupService>();
        IDatabaseInitializer initializer = _provider.GetRequiredService<IDatabaseInitializer>();

        await startup.InitializeAsync();
        await startup.InitializeAsync();

        await initializer.InitializeAsync();

        IQuotService quotService = _provider.GetRequiredService<IQuotService>();
        IEnumerable<PsychologyApp.Application.Models.QuotDTO> quotes = await quotService.GetAllAsync(5);

        Assert.NotEmpty(quotes);
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
