using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.Body;
using PsychologyApp.Application.DataBackup;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.Body;

/// <summary>Body check-ins through the real services and SQLite: an entry is kept with its note, and it travels with a backup once.</summary>
public sealed class BodyIntegrationTests : IAsyncLifetime
{
    private readonly SharedMemoryConnectionFactory _sourceFactory = new();
    private readonly SharedMemoryConnectionFactory _targetFactory = new();
    private ServiceProvider _source = null!;
    private ServiceProvider _target = null!;

    public Task InitializeAsync()
    {
        _source = IntegrationTestServiceCollection.BuildCoreProvider(_sourceFactory);
        _target = IntegrationTestServiceCollection.BuildCoreProvider(_targetFactory);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task A_check_in_is_stored_and_restored_from_a_backup_without_doubles()
    {
        IBodyService source = _source.GetRequiredService<IBodyService>();
        await source.SaveAsync(BodyArea.Throat, BodySensation.Lump, 6, [BodyFeeling.Hurt], "after the call with mum");

        BodyEntry stored = Assert.Single((await source.GetLogAsync()).Entries);
        Assert.Equal((BodyArea.Throat, BodySensation.Lump, "after the call with mum"), (stored.Area, stored.Sensation, stored.Note));

        string json = await _source.GetRequiredService<IBackupService>().ExportAsync();
        IBackupService target = _target.GetRequiredService<IBackupService>();
        await target.ImportAsync(json);
        await target.ImportAsync(json);

        BodyEntry restored = Assert.Single((await _target.GetRequiredService<IBodyService>().GetLogAsync()).Entries);
        Assert.Equal(stored, restored with { Feelings = stored.Feelings });
        Assert.Equal([BodyFeeling.Hurt], restored.Feelings);
    }

    public async Task DisposeAsync()
    {
        await _sourceFactory.DisposeAsync();
        await _targetFactory.DisposeAsync();
        await _source.DisposeAsync();
        await _target.DisposeAsync();
    }
}
