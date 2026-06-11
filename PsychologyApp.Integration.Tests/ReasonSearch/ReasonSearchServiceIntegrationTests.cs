using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Services.ReasonSearch;
using PsychologyApp.Domain.Entities;
using PsychologyApp.Testing.Data;
using PsychologyApp.Testing.Integration;
using Xunit;

namespace PsychologyApp.Integration.Tests.ReasonSearch;

public sealed class ReasonSearchServiceIntegrationTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private ServiceProvider _provider = null!;

    public Task InitializeAsync()
    {
        var reasons = new[]
        {
            Reason.Create("Тревога", "Психосоматика", "Дыхательные практики"),
            Reason.Create("Головная боль", "Соматика", "Отдых и вода")
        };

        _provider = IntegrationTestServiceCollection.BuildCoreProvider(_connectionFactory, services =>
        {
            IntegrationTestServiceCollection.ReplaceSingleton<IReasonContentProvider>(
                services,
                new FakeReasonContentProvider(reasons));
        });

        return Task.CompletedTask;
    }

    [Fact]
    public async Task LoadReasonsAndSearch_FindsMatchFromStubbedContentProvider()
    {
        IReasonSearchService search = _provider.GetRequiredService<IReasonSearchService>();

        IReadOnlyList<PsychologyApp.Application.Models.ReasonDTO> loaded = await search.LoadReasonsAsync();
        IReadOnlyList<RankedReason> results = search.Search(loaded, "тревога");

        Assert.Equal(2, loaded.Count);
        Assert.Single(results);
        Assert.Equal("Тревога", results[0].Reason.Title);
        Assert.True(results[0].MatchScore > 0);
    }

    public async Task DisposeAsync()
    {
        await _connectionFactory.DisposeAsync();
        await _provider.DisposeAsync();
    }
}
