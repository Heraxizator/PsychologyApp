using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.ClinicalCare;
using System.Text;

namespace PsychologyApp.Application.ClinicalCare;

public sealed class SpecialistSummaryService(
    IUserProgressService userProgressService,
    IClinicalCareService clinicalCareService,
    IChatRepository chatRepository) : ISpecialistSummaryService
{
    private const int PeriodDays = 30;

    public async Task<string> BuildSummaryAsync(string? language, CancellationToken cancellationToken = default)
    {
        DateTime nowUtc = DateTime.UtcNow;
        DateTime sinceUtc = nowUtc.AddDays(-PeriodDays);

        IReadOnlyList<MoodEntryDTO> moods = await userProgressService.GetMoodsAsync(sinceUtc, nowUtc, 1000, cancellationToken).ConfigureAwait(false);
        IReadOnlyList<CompletionDTO> completions = await userProgressService.GetRecentTechniqueCompletionsAsync(1000, cancellationToken).ConfigureAwait(false);
        int practiceCount = completions.Count(entry => entry.CompletedAt >= sinceUtc);
        int streakDays = await userProgressService.GetStreakDaysAsync(cancellationToken).ConfigureAwait(false);

        IReadOnlyList<TestResultDTO> allTests = await userProgressService.GetAllTestResultsAsync(1000, cancellationToken).ConfigureAwait(false);
        List<TestResultDTO> latestPerTest = allTests
            .Where(result => result.CompletedAt >= sinceUtc)
            .GroupBy(result => result.TestId)
            .Select(group => group.OrderByDescending(result => result.CompletedAt).First())
            .OrderByDescending(result => result.CompletedAt)
            .ToList();

        RiskAssessmentDTO? latestRisk = await clinicalCareService.GetLatestRiskAssessmentAsync(cancellationToken).ConfigureAwait(false);
        SafetyPlanDTO safetyPlan = await clinicalCareService.GetSafetyPlanAsync(cancellationToken).ConfigureAwait(false);

        SpecialistSummaryText text = SpecialistSummaryText.For(language);
        StringBuilder sb = new();
        sb.AppendLine(text.Title);
        sb.AppendLine(string.Format(text.Period, PeriodDays, nowUtc.ToLocalTime()));
        sb.AppendLine();

        sb.AppendLine(text.PracticeHeader);
        sb.AppendLine(string.Format(text.PracticeLine, practiceCount, streakDays));
        sb.AppendLine();

        sb.AppendLine(text.MoodHeader);
        sb.AppendLine(moods.Count == 0
            ? text.NoMood
            : string.Format(text.MoodLine, moods.Count, moods.Average(entry => entry.MoodLevel)));
        sb.AppendLine();

        sb.AppendLine(text.TestsHeader);
        if (latestPerTest.Count == 0)
        {
            sb.AppendLine(text.NoTests);
        }
        else
        {
            foreach (TestResultDTO result in latestPerTest)
            {
                string scoreText = result.Score is int score ? $"{score}" : "—";
                sb.AppendLine(string.Format(text.TestLine, result.TestId, scoreText, result.Summary, result.CompletedAt.ToLocalTime()));
            }
        }
        sb.AppendLine();

        sb.AppendLine(text.RiskHeader);
        sb.AppendLine(latestRisk is null
            ? text.NoRisk
            : string.Format(text.LastRisk, DescribeRisk(latestRisk.RiskLevel, text), latestRisk.AssessedAt.ToLocalTime()));
        sb.AppendLine();

        sb.AppendLine(text.SafetyHeader);
        sb.AppendLine(safetyPlan.IsEmpty
            ? text.PlanEmpty
            : string.Format(text.PlanFilled, safetyPlan.WarningSigns.Count, safetyPlan.CopingStrategies.Count, safetyPlan.Contacts.Count));
        sb.AppendLine();

        // What the trauma conversations found: beliefs and triggers in the person's words, without details of the event.
        // The trauma chat speaks only Russian and English, so its notes are worded in one of them.
        bool english = !string.Equals(language, "ru", StringComparison.OrdinalIgnoreCase);
        IReadOnlyDictionary<string, string> memory = await chatRepository.GetMemoryAsync(cancellationToken).ConfigureAwait(false);
        TraumaNotes notes = TraumaNotes.Deserialize(memory.TryGetValue(ChatMemoryKeys.TraumaNotes, out string? stored) ? stored : null);
        if (!notes.IsEmpty)
        {
            sb.AppendLine(CompanionTraumaContent.SummaryTitle(english));
            foreach (StuckPointNote stuck in notes.StuckPoints)
            {
                sb.AppendLine(CompanionTraumaContent.SummaryStuckPoint(stuck, english));
            }

            foreach (TriggerNote trigger in notes.Triggers)
            {
                sb.AppendLine(CompanionTraumaContent.SummaryTrigger(trigger, english));
            }

            sb.AppendLine();
        }

        sb.AppendLine(text.Footer);

        return sb.ToString();
    }

    private static string DescribeRisk(RiskLevel level, SpecialistSummaryText text) => level switch
    {
        RiskLevel.Red => text.RiskHigh,
        RiskLevel.Amber => text.RiskModerate,
        _ => text.RiskLow
    };
}
