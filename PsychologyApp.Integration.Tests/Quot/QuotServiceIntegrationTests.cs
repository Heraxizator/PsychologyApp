using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Quot;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.Quot;

public sealed class QuotServiceIntegrationTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private ServiceProvider _provider = null!;

    public Task InitializeAsync()
    {
        _provider = IntegrationTestServiceCollection.BuildCoreProvider(_connectionFactory, services =>
        {
            var inner = new FakeQuotContentProvider();
            IntegrationTestServiceCollection.ReplaceSingleton<IQuotContentProvider>(
                services,
                new CachedQuotContentProvider(inner));
        });
        return Task.CompletedTask;
    }

    [Fact]
    public async Task LoadSingleAsync_PersistsQuoteToDatabase()
    {
        IDatabaseInitializer initializer = _provider.GetRequiredService<IDatabaseInitializer>();
        IQuotService quotService = _provider.GetRequiredService<IQuotService>();

        await initializer.InitializeAsync();
        await quotService.LoadSingleAsync();

        IEnumerable<PsychologyApp.Application.Models.QuotDTO> quotes =
            await quotService.GetAllAsync(5);

        Assert.NotEmpty(quotes);
    }

    public async Task DisposeAsync()
    {
        await _connectionFactory.DisposeAsync();
        await _provider.DisposeAsync();
    }

    private sealed class FakeQuotContentProvider : IQuotContentProvider
    {
        public Task<IReadOnlyList<QuotSeed>> LoadAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<QuotSeed>>(
            [
                new QuotSeed("Title", "Text", "Theme")
            ]);
    }
}
