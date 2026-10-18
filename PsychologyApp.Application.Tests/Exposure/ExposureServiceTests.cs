using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Exposure;
using PsychologyApp.Domain.Exposure;
using Xunit;

namespace PsychologyApp.Application.Tests.Exposure;

public sealed class ExposureServiceTests
{
    private static readonly ExposureScreeningAnswers NoRisk = new(false, false, false, false, false, false);

    private sealed class MemoryRepository : IExposureRepository
    {
        public ExposurePlan? Plan { get; private set; }

        public Task<ExposurePlan?> GetAsync(CancellationToken cancellationToken = default) => Task.FromResult(Plan);

        public Task SaveAsync(ExposurePlan plan, CancellationToken cancellationToken = default)
        {
            Plan = plan;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            Plan = null;
            return Task.CompletedTask;
        }
    }

    private sealed class Clock(DateTime start) : TimeProvider
    {
        public DateTime Now { get; set; } = start;

        public override DateTimeOffset GetUtcNow() => new(Now, TimeSpan.Zero);
    }

    private readonly MemoryRepository _repository = new();
    private readonly Clock _clock = new(new DateTime(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc));
    private readonly ExposureService _service;

    public ExposureServiceTests() => _service = new ExposureService(_repository, new KeywordCrisisDetector(), _clock);

    private async Task<ExposureStep> LadderWithOneStepAsync()
    {
        await _service.ScreenAsync(NoRisk);
        ExposurePlan plan = await _service.SaveStepsAsync([new ExposureStep(string.Empty, "Call the clinic", 40)]);
        return plan.Steps[0];
    }

    [Fact]
    public async Task The_first_screening_creates_the_plan_with_its_result_and_date()
    {
        ExposureScreeningResult result = await _service.ScreenAsync(NoRisk with { BodyRisk = true });

        Assert.Equal(ExposureScreeningResult.AllowedWithoutBodyExercises, result);
        Assert.Equal(ExposureScreeningResult.AllowedWithoutBodyExercises, _repository.Plan!.Screening);
        Assert.Equal(_clock.Now, _repository.Plan.ScreenedAt);
    }

    [Fact]
    public async Task Steps_get_ids_ratings_are_kept_in_range_and_empty_steps_are_dropped()
    {
        ExposurePlan plan = await _service.SaveStepsAsync(
        [
            new ExposureStep(string.Empty, "  Call the clinic  ", 140),
            new ExposureStep(string.Empty, "   ", 20),
            new ExposureStep("kept", "Ask a question", -5)
        ]);

        Assert.Equal(2, plan.Steps.Count);
        Assert.False(string.IsNullOrEmpty(plan.Steps[0].Id));
        Assert.Equal("Call the clinic", plan.Steps[0].Text);
        Assert.Equal(100, plan.Steps[0].ExpectedDistress);
        Assert.Equal(("kept", 0), (plan.Steps[1].Id, plan.Steps[1].ExpectedDistress));
    }

    [Fact]
    public async Task Rewriting_the_ladder_keeps_the_tasks_attached_to_their_step()
    {
        ExposureStep step = await LadderWithOneStepAsync();
        await _service.StartTaskAsync(step.Id, "They'll hear my voice shake", 80, []);

        ExposurePlan plan = await _service.SaveStepsAsync([step with { Text = "Call the clinic and ask about prices" }]);

        Assert.Single(ExposureLadder.SessionsOf(plan, step.Id));
    }

    [Fact]
    public async Task A_task_is_pending_until_reported_and_the_report_records_the_outcome()
    {
        ExposureStep step = await LadderWithOneStepAsync();
        ExposureSession started = await _service.StartTaskAsync(step.Id, "They'll hear my voice shake", 80, ["Rehearsing what I'll say"]);
        Assert.Equal(started.Id, ExposureLadder.Pending(_repository.Plan!)!.Id);

        _clock.Now = _clock.Now.AddHours(2);
        ExposureReportResult result = await _service.ReportTaskAsync(
            started.Id,
            new ExposureReport("They just booked me in", PredictionOutcome.NotHappened, 25, 60, false, "Nobody noticed anything"));

        ExposureSession reported = Assert.Single(result.Plan.Sessions);
        Assert.False(result.Crisis);
        Assert.Null(ExposureLadder.Pending(result.Plan));
        Assert.Equal(PredictionOutcome.NotHappened, reported.Outcome);
        Assert.Equal(55, ExposureLadder.BeliefDrop(reported));
        Assert.Equal(_clock.Now, reported.ReportedAt);
        Assert.Equal(["Rehearsing what I'll say"], reported.DroppedSafetyBehaviours);
    }

    [Fact]
    public async Task A_report_that_reads_as_a_crisis_is_flagged()
    {
        ExposureStep step = await LadderWithOneStepAsync();
        ExposureSession started = await _service.StartTaskAsync(step.Id, "They'll laugh", 70, []);

        ExposureReportResult result = await _service.ReportTaskAsync(
            started.Id,
            new ExposureReport("It went badly", PredictionOutcome.Happened, 90, 95, true, "I want to kill myself"));

        Assert.True(result.Crisis);
    }

    [Fact]
    public async Task An_interrupted_task_is_reported_without_an_outcome()
    {
        ExposureStep step = await LadderWithOneStepAsync();
        ExposureSession started = await _service.StartTaskAsync(step.Id, "I'll faint", 90, []);

        ExposurePlan plan = await _service.InterruptTaskAsync(started.Id, 95);

        ExposureSession session = Assert.Single(plan.Sessions);
        Assert.True(session.Interrupted);
        Assert.True(session.IsReported);
        Assert.Null(session.Outcome);
        Assert.Equal(95, session.PeakDistress);
    }

    [Fact]
    public async Task A_task_cannot_start_on_a_step_that_is_not_on_the_ladder()
    {
        await LadderWithOneStepAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.StartTaskAsync("missing", "x", 50, []));
    }

    [Fact]
    public async Task Concurrent_changes_are_not_lost()
    {
        ExposureStep step = await LadderWithOneStepAsync();

        await Task.WhenAll(Enumerable.Range(0, 10).Select(i => _service.StartTaskAsync(step.Id, $"p{i}", 50, [])));

        Assert.Equal(10, _repository.Plan!.Sessions.Count);
    }

    [Fact]
    public async Task Deleting_the_plan_removes_everything()
    {
        await LadderWithOneStepAsync();

        await _service.DeletePlanAsync();

        Assert.Null(await _service.GetPlanAsync());
    }
}
