using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.ClinicalCare;

public sealed class ClinicalCareServiceTests
{
    [Theory]
    [InlineData(true, false, false, false, RiskLevel.Red)]
    [InlineData(false, true, false, false, RiskLevel.Red)]
    [InlineData(false, false, true, false, RiskLevel.Red)]
    [InlineData(false, false, false, true, RiskLevel.Amber)]
    [InlineData(false, false, false, false, RiskLevel.Green)]
    public void ClassifyRisk_ReturnsExpectedLevel(
        bool selfHarm,
        bool disorientation,
        bool substance,
        bool insomnia,
        RiskLevel expected)
    {
        RiskLevel actual = ClinicalCareService.ClassifyRisk(new RiskAssessmentInput
        {
            HasSelfHarmThoughts = selfHarm,
            HasSevereDisorientation = disorientation,
            HasSubstanceRisk = substance,
            HasSevereInsomnia = insomnia
        });

        Assert.Equal(expected, actual);
    }

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
        public TherapyProgramStateDTO? ActiveProgram { get; private set; }

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
    }

    private sealed class FakeUserProgressService : IUserProgressService
    {
        public Task SaveTestResultAsync(string testId, int? score, string summary, string? detailJson = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<TestResultDTO?> GetLatestTestResultAsync(string testId, CancellationToken cancellationToken = default) => Task.FromResult<TestResultDTO?>(null);
        public Task<TestResultDTO?> GetMostRecentTestResultAsync(TimeSpan within, CancellationToken cancellationToken = default) => Task.FromResult<TestResultDTO?>(null);
        public Task<IReadOnlyList<TestResultDTO>> GetTestResultHistoryAsync(string testId, int limit = 20, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<TestResultDTO>>([]);
        public Task<IReadOnlyDictionary<string, TestResultDTO>> GetLatestTestResultsAsync(IReadOnlyList<string> testIds, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyDictionary<string, TestResultDTO>>(new Dictionary<string, TestResultDTO>());
        public Task<IReadOnlyDictionary<string, int>> GetTestResultCountsAsync(IReadOnlyList<string> testIds, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyDictionary<string, int>>(new Dictionary<string, int>());
        public Task<long> CountTestResultsAsync(CancellationToken cancellationToken = default) => Task.FromResult(0L);
        public Task<DateTime?> GetLastTechniqueCompletionDateAsync(CancellationToken cancellationToken = default) => Task.FromResult<DateTime?>(null);
        public Task RecordTechniqueCompletionAsync(string itemKey, string moduleName, string pageName, int durationSeconds = 0, CancellationToken cancellationToken = default) => Task.CompletedTask;
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
        public Task RecordMoodAsync(int moodLevel, string? note = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<IReadOnlyList<MoodEntryDTO>> GetRecentMoodsAsync(int limit = 7, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<MoodEntryDTO>>([]);
    }
}
