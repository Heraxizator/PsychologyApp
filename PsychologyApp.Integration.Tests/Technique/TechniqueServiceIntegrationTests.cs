using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Technique;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.Technique;

public sealed class TechniqueServiceIntegrationTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private ServiceProvider _provider = null!;

    public Task InitializeAsync()
    {
        _provider = IntegrationTestServiceCollection.BuildCoreProvider(_connectionFactory);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AddNewTechniqueAsync_GetTechniqueByIdAsync_RoundTripsThroughServiceAndRepository()
    {
        ITechniqueService service = _provider.GetRequiredService<ITechniqueService>();
        ITechniqueRepository repository = _provider.GetRequiredService<ITechniqueRepository>();

        var dto = new TechniqueDTO
        {
            TechniqueId = 0,
            Number = "1",
            Date = "2026-06-06",
            Header = "Integration Header",
            Description = "Description",
            Subject = "Subject",
            Author = "Author",
            Algorithm = "Step 1",
            Image = "img.png"
        };

        await service.AddNewTechniqueAsync(dto);

        TechniqueDTO latest = (await service.GetTechniquesListAsync(1)).Single();
        TechniqueDTO loaded = await service.GetTechniqueByIdAsync(latest.TechniqueId);

        Assert.Equal("Integration Header", loaded.Header);
        Assert.Equal("Author", loaded.Author);

        var persisted = await repository.GetByIdAsync(latest.TechniqueId);
        Assert.NotNull(persisted);
        Assert.Equal("Integration Header", persisted!.Header);
    }

    public async Task DisposeAsync()
    {
        await _connectionFactory.DisposeAsync();
        await _provider.DisposeAsync();
    }
}
