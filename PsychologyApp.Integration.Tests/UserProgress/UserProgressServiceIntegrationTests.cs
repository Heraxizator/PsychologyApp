using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.UserProgress;

public sealed class UserProgressServiceIntegrationTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _connectionFactory = new();
    private ServiceProvider _provider = null!;

    public Task InitializeAsync()
    {
        _provider = IntegrationTestServiceCollection.BuildCoreProvider(_connectionFactory);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task RecordTechniqueCompletionAsync_PersistsAndCountsThroughService()
    {
        IUserProgressService progress = _provider.GetRequiredService<IUserProgressService>();
        const string itemKey = "technique-spin";

        long before = await progress.CountTechniqueCompletionsAsync();
        await progress.RecordTechniqueCompletionAsync(itemKey, "Practice", "Spin", durationSeconds: 30);

        long after = await progress.CountTechniqueCompletionsAsync();
        DateTime? lastPractice = await progress.GetLastPracticeDateAsync(itemKey);

        Assert.Equal(before + 1, after);
        Assert.NotNull(lastPractice);
    }

    public async Task DisposeAsync()
    {
        await _connectionFactory.DisposeAsync();
        await _provider.DisposeAsync();
    }
}
