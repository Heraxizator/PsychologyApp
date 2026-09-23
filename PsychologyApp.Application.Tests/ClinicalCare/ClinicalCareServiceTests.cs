using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.ClinicalCare;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.ClinicalCare;

public sealed class ClinicalCareServiceTests
{
    [Fact]
    public async Task AssessRiskAsync_Red_SavesCrisisEscalation()
    {
        var repo = new FakeClinicalCareRepository();
        var service = new ClinicalCareService(repo, new FakeUserProgressService());

        RiskAssessmentDTO result = await service.AssessRiskAsync(new RiskAssessmentInput
        {
            HasSelfHarmThoughts = true,
            Source = "test"
        });

        Assert.Equal(RiskLevel.Red, result.RiskLevel);
        Assert.True(await service.ShouldRouteToCrisisHubAsync());
        Assert.Contains(repo.Escalations, e => e.Action == EscalationActions.RouteToCrisisHub);
    }

    [Fact]
    public async Task AssessRiskAsync_Amber_SavesSpecialistEscalation()
    {
        var repo = new FakeClinicalCareRepository();
        var service = new ClinicalCareService(repo, new FakeUserProgressService());

        RiskAssessmentDTO result = await service.AssessRiskAsync(new RiskAssessmentInput
        {
            HasSevereInsomnia = true,
            Source = "test"
        });

        Assert.Equal(RiskLevel.Amber, result.RiskLevel);
        Assert.False(await service.ShouldRouteToCrisisHubAsync());
        Assert.Contains(repo.Escalations, e => e.Action == EscalationActions.OfferSpecialistHelp);
    }

    [Fact]
    public async Task EnsureProgramAsync_CreatesAnxietyProgramFromConcern()
    {
        var repo = new FakeClinicalCareRepository();
        var service = new ClinicalCareService(repo, new FakeUserProgressService());

        TherapyProgramStateDTO program = await service.EnsureProgramAsync(OnboardingConcernKeys.Anxiety);

        Assert.Equal(TherapyProgramType.Anxiety, program.ProgramType);
        Assert.Equal(1, program.CurrentWeek);
        Assert.True(program.IsActive);
    }

    [Fact]
    public async Task IsRiskCheckDueAsync_WhenNoAssessment_ReturnsTrue()
    {
        var service = new ClinicalCareService(new FakeClinicalCareRepository(), new FakeUserProgressService());
        Assert.True(await service.IsRiskCheckDueAsync(TimeSpan.FromDays(7)));
    }

    [Fact]
    public async Task AdjustProgramFromScorecardAsync_WhenNoProgram_ReturnsNull()
    {
        var service = new ClinicalCareService(new FakeClinicalCareRepository(), new FakeUserProgressService());
        Assert.Null(await service.AdjustProgramFromScorecardAsync());
    }

    [Fact]
    public async Task AdjustProgramFromScorecardAsync_WhenRedRisk_EscalatesWithoutChangingWeek()
    {
        var repo = new FakeClinicalCareRepository();
        repo.ActiveProgram = new TherapyProgramStateDTO
        {
            ProgramType = TherapyProgramType.Anxiety,
            StartedAt = DateTime.UtcNow.AddDays(-3),
            CurrentWeek = 2,
            IsActive = true
        };
        repo.Assessments.Add(new RiskAssessmentDTO
        {
            AssessedAt = DateTime.UtcNow,
            RiskLevel = RiskLevel.Red,
            Source = "test"
        });
        var service = new ClinicalCareService(repo, new FakeUserProgressService());

        TherapyProgramStateDTO? result = await service.AdjustProgramFromScorecardAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.CurrentWeek);
        Assert.Contains(repo.Escalations, e =>
            e.Action == EscalationActions.RouteToCrisisHub &&
            e.TriggerSource == "weekly_scorecard");
    }

    [Fact]
    public async Task AdjustProgramFromScorecardAsync_WhenAmberRisk_HoldsWeekAndOffersHelp()
    {
        var repo = new FakeClinicalCareRepository();
        repo.ActiveProgram = new TherapyProgramStateDTO
        {
            ProgramType = TherapyProgramType.Mood,
            StartedAt = DateTime.UtcNow.AddDays(-10),
            CurrentWeek = 3,
            IsActive = true
        };
        repo.Assessments.Add(new RiskAssessmentDTO
        {
            AssessedAt = DateTime.UtcNow,
            RiskLevel = RiskLevel.Amber,
            Source = "test"
        });
        var service = new ClinicalCareService(repo, new FakeUserProgressService());

        TherapyProgramStateDTO? result = await service.AdjustProgramFromScorecardAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.CurrentWeek);
        Assert.Contains(repo.Escalations, e => e.Action == EscalationActions.OfferSpecialistHelp);
    }

    [Fact]
    public async Task AdjustProgramFromScorecardAsync_WhenGreen_AdvancesWeekIfDue()
    {
        var repo = new FakeClinicalCareRepository();
        repo.ActiveProgram = new TherapyProgramStateDTO
        {
            ProgramType = TherapyProgramType.Stress,
            StartedAt = DateTime.UtcNow.AddDays(-14),
            CurrentWeek = 1,
            IsActive = true
        };
        var service = new ClinicalCareService(repo, new FakeUserProgressService());

        TherapyProgramStateDTO? result = await service.AdjustProgramFromScorecardAsync();

        Assert.NotNull(result);
        Assert.True(result.CurrentWeek >= 2);
        Assert.Empty(repo.Escalations);
    }

    [Fact]
    public void TherapyProgramCatalog_HasPoolsForAllPrograms()
    {
        foreach (TherapyProgramType type in Enum.GetValues<TherapyProgramType>())
        {
            for (int week = 1; week <= TherapyProgramCatalog.TotalWeeks; week++)
            {
                TherapyProgramWeekPlan plan = TherapyProgramCatalog.GetWeekPlan(type, week);
                Assert.NotEmpty(plan.TechniquePool);
                Assert.Equal(week, plan.Week);
            }
        }
    }

    private sealed class FakeClinicalCareRepository : IClinicalCareRepository
    {
        public List<RiskAssessmentDTO> Assessments { get; } = [];
        public List<EscalationEventDTO> Escalations { get; } = [];
        public TherapyProgramStateDTO? ActiveProgram { get; set; }

        public Task SaveRiskAssessmentAsync(RiskAssessmentDTO assessment, CancellationToken cancellationToken = default)
        {
            Assessments.Add(assessment);
            return Task.CompletedTask;
        }

        public Task<RiskAssessmentDTO?> GetLatestRiskAssessmentAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Assessments.LastOrDefault());

        public Task UpsertActiveProgramAsync(TherapyProgramStateDTO program, CancellationToken cancellationToken = default)
        {
            ActiveProgram = program;
            return Task.CompletedTask;
        }

        public Task<TherapyProgramStateDTO?> GetActiveProgramAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(ActiveProgram);

        public Task SaveEscalationEventAsync(EscalationEventDTO escalation, CancellationToken cancellationToken = default)
        {
            Escalations.Add(escalation);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<EscalationEventDTO>> GetRecentEscalationsAsync(int limit, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<EscalationEventDTO>>(Escalations.Take(limit).ToList());

        public SafetyPlanDTO? SafetyPlan { get; set; }

        public Task<SafetyPlanDTO?> GetSafetyPlanAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(SafetyPlan);

        public Task SaveSafetyPlanAsync(SafetyPlanDTO plan, CancellationToken cancellationToken = default)
        {
            SafetyPlan = plan;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUserProgressService : IUserProgressService
    {
        public Task SaveTestResultAsync(string testId, int? score, string summary, string? detailJson = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default) => Task.FromResult<TestResultDTO?>(null);
        public Task<TestResultDTO?> GetMostRecentTestResultAsync(TimeSpan within, CancellationToken cancellationToken = default) => Task.FromResult<TestResultDTO?>(null);
        public Task<IReadOnlyList<TestResultDTO>> GetTestResultHistoryAsync(string testId, int limit = 20, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<TestResultDTO>>([]);
        public Task<IReadOnlyList<TestResultDTO>> GetAllTestResultsAsync(int limit = 10000, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<TestResultDTO>>([]);
        public Task<IReadOnlyDictionary<string, TestResultDTO>> GetLatestTestResultsAsync(IReadOnlyList<string> testIds, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyDictionary<string, TestResultDTO>>(new Dictionary<string, TestResultDTO>());
        public Task<IReadOnlyDictionary<string, int>> GetTestResultCountsAsync(IReadOnlyList<string> testIds, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyDictionary<string, int>>(new Dictionary<string, int>());
        public Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default) => Task.FromResult(0L);
        public Task<DateTime?> GetLastTechniqueCompletionDateAsync(CancellationToken cancellationToken = default) => Task.FromResult<DateTime?>(null);
        public Task RecordTechniqueCompletionAsync(string itemKey, string moduleName, string pageName, int durationSeconds = 0, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<long> RecordSessionOutcomeAsync(SessionOutcomeRequest request, CancellationToken cancellationToken = default) => Task.FromResult(1L);
        public Task<long> CountTechniqueCompletionsAsync(CancellationToken cancellationToken = default) => Task.FromResult(0L);
        public Task<IReadOnlyList<CompletionDTO>> GetRecentTechniqueCompletionsAsync(int limit = 20, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<CompletionDTO>>([]);
        public Task<int> GetStreakDaysAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<int> GetAtRiskStreakDaysAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<DateTime?> GetLastPracticeDateAsync(string itemKey, CancellationToken cancellationToken = default) => Task.FromResult<DateTime?>(null);
        public Task<IReadOnlyDictionary<string, DateTime>> GetLastPracticeDatesAsync(IReadOnlyList<string> itemKeys, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyDictionary<string, DateTime>>(new Dictionary<string, DateTime>());
        public Task SaveSessionDraftAsync(string techniqueKey, string payloadJson, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<string?> GetSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);
        public Task<IReadOnlySet<string>> GetSessionDraftKeysAsync(IReadOnlyList<string> techniqueKeys, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlySet<string>>(new HashSet<string>());
        public Task DeleteSessionDraftAsync(string techniqueKey, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RecordMoodAsync(int moodLevel, string? note = null, DateTime? recordedAtUtc = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateMoodEntryAsync(long moodEntryId, int moodLevel, string? note = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeleteMoodEntryAsync(long moodEntryId, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IReadOnlyList<MoodEntryDTO>> GetRecentMoodsAsync(int limit = 7, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<MoodEntryDTO>>([]);
        public Task<IReadOnlyList<MoodEntryDTO>> GetMoodsAsync(DateTime? fromUtc = null, DateTime? toUtc = null, int limit = 60, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<MoodEntryDTO>>([]);
        public Task UpdateSessionResultPostIntensityAsync(long sessionResultId, int postIntensity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<SessionResultDTO?> GetSessionResultAsync(long sessionResultId, CancellationToken cancellationToken = default) => Task.FromResult<SessionResultDTO?>(null);
        public Task<IReadOnlyList<SessionResultDTO>> GetRecentSessionResultsAsync(int limit = 20, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<SessionResultDTO>>([]);
        public Task<int> CountDistinctTechniqueCompletionsForItemsBetweenAsync(IReadOnlyList<string> itemKeys, DateTime sinceUtc, DateTime beforeUtc, CancellationToken cancellationToken = default) => Task.FromResult(0);
    }
}
