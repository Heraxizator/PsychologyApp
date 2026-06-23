using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Technique;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.Bootstrap;

public sealed class DatabaseInitializerIntegrationTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private ServiceProvider _provider = null!;

    public Task InitializeAsync()
    {
        _provider = IntegrationTestServiceCollection.BuildCoreProvider(_connectionFactory);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AddPsychologyAppCore_WithInMemoryDatabase_InitializesSchemaAndResolvesServices()
    {
        IDatabaseInitializer initializer = _provider.GetRequiredService<IDatabaseInitializer>();
        ITechniqueService techniqueService = _provider.GetRequiredService<ITechniqueService>();

        await initializer.InitializeAsync();

        IEnumerable<PsychologyApp.Application.Models.TechniqueDTO> techniques =
            await techniqueService.GetTechniquesListAsync(1);

        Assert.NotNull(techniques);
    }

    public async Task DisposeAsync()
    {
        await _connectionFactory.DisposeAsync();
        await _provider.DisposeAsync();
    }
}
