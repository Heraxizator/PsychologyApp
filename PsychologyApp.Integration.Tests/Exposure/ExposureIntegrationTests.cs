using Microsoft.Extensions.DependencyInjection;
using PsychologyApp.Application.DataBackup;
using PsychologyApp.Application.Exposure;
using PsychologyApp.Domain.Exposure;
using PsychologyApp.Testing.Data;
using Xunit;

namespace PsychologyApp.Integration.Tests.Exposure;

/// <summary>The fear ladder through the real services and SQLite: every recorded task stays, and it travels with a backup.</summary>
public sealed class ExposureIntegrationTests : IAsyncLifetime
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

    private static async Task<ExposurePlan> WalkOneTaskAsync(IExposureService service)
    {
        await service.ScreenAsync(new ExposureScreeningAnswers(false, false, false, false, false, false));
        await service.SaveFormulationAsync(FearKind.Social, "They'll laugh at me", 80, ["Phone calls"], ["Rehearsing what I'll say"]);
        ExposurePlan plan = await service.SaveStepsAsync([new ExposureStep(string.Empty, "Call the clinic", 40)]);
        ExposureSession task = await service.StartTaskAsync(plan.Steps[0].Id, "My voice will shake and they'll notice", 75, ["Rehearsing what I'll say"]);
        ExposureReportResult result = await service.ReportTaskAsync(
            task.Id,
            new ExposureReport("They booked me in", PredictionOutcome.NotHappened, 30, 55, false, "Nobody noticed anything"));
        return result.Plan;
    }

    [Fact]
    public async Task A_walked_task_is_stored_and_read_back_whole()
    {
        ExposurePlan written = await WalkOneTaskAsync(_source.GetRequiredService<IExposureService>());

        ExposurePlan? read = await _source.GetRequiredService<IExposureService>().GetPlanAsync();

        Assert.NotNull(read);
        Assert.Equal(FearKind.Social, read.Kind);
        Assert.Equal(ExposureScreeningResult.Allowed, read.Screening);
        Assert.Equal(written.Steps, read.Steps);
        ExposureSession session = Assert.Single(read.Sessions);
        Assert.Equal(PredictionOutcome.NotHappened, session.Outcome);
        Assert.Equal(["Rehearsing what I'll say"], session.DroppedSafetyBehaviours);
        Assert.Equal(45, ExposureLadder.BeliefDrop(session));
    }

    [Fact]
    public async Task The_ladder_travels_with_a_backup_and_does_not_overwrite_a_newer_one()
    {
        await WalkOneTaskAsync(_source.GetRequiredService<IExposureService>());
        string json = await _source.GetRequiredService<IBackupService>().ExportAsync();
        IExposureService target = _target.GetRequiredService<IExposureService>();

        await _target.GetRequiredService<IBackupService>().ImportAsync(json);
        ExposurePlan restored = (await target.GetPlanAsync())!;
        Assert.Single(restored.Sessions);

        // Work done after the restore must survive importing the same (older) file again.
        ExposureSession more = await target.StartTaskAsync(restored.Steps[0].Id, "They'll hang up", 50, []);
        await _target.GetRequiredService<IBackupService>().ImportAsync(json);

        Assert.Contains((await target.GetPlanAsync())!.Sessions, session => session.Id == more.Id);
    }

    public async Task DisposeAsync()
    {
        await _sourceFactory.DisposeAsync();
        await _targetFactory.DisposeAsync();
        await _source.DisposeAsync();
        await _target.DisposeAsync();
    }
}
