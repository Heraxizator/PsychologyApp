using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Technique;
using PsychologyApp.Bootstrap;
using Xunit;

namespace PsychologyApp.Application.Tests.Bootstrap;

public class BootstrapTests
{
    private sealed class TestDatabaseKeyProvider : IDatabaseKeyProvider
    {
        public Task<string?> GetKeyAsync(CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);

        public Task<string?> CreateKeyAsync(CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);
    }

    [Fact]
    public void AddPsychologyAppCore_RegistersTechniqueService()
    {
        ServiceCollection services = new();
        services.AddSingleton<IDatabaseKeyProvider, TestDatabaseKeyProvider>();
        services.AddPsychologyAppCore();
        ServiceProvider provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetService<ITechniqueService>());
        Assert.NotNull(provider.GetService<ITechniqueCatalogProvider>());
    }
}
