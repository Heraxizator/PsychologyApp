using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.ClinicalCare;
using System.Text;

namespace PsychologyApp.Application.ClinicalCare;

public sealed class SpecialistSummaryService(
    IUserProgressService userProgressService,
    IClinicalCareService clinicalCareService) : ISpecialistSummaryService
{
    private const int PeriodDays = 30;

    public async Task<string> BuildSummaryAsync(bool english, CancellationToken cancellationToken = default)
    {
        DateTime nowUtc = DateTime.UtcNow;
        DateTime sinceUtc = nowUtc.AddDays(-PeriodDays);

        IReadOnlyList<MoodEntryDTO> moods = await userProgressService.GetMoodsAsync(sinceUtc, nowUtc, 1000, cancellationToken);
        IReadOnlyList<CompletionDTO> completions = await userProgressService.GetRecentTechniqueCompletionsAsync(1000, cancellationToken);
        int practiceCount = completions.Count(entry => entry.CompletedAt >= sinceUtc);
        int streakDays = await userProgressService.GetStreakDaysAsync(cancellationToken);

        IReadOnlyList<TestResultDTO> allTests = await userProgressService.GetAllTestResultsAsync(1000, cancellationToken);
        List<TestResultDTO> latestPerTest = allTests
            .Where(result => result.CompletedAt >= sinceUtc)
            .GroupBy(result => result.TestId)
            .Select(group => group.OrderByDescending(result => result.CompletedAt).First())
            .OrderByDescending(result => result.CompletedAt)
            .ToList();

        RiskAssessmentDTO? latestRisk = await clinicalCareService.GetLatestRiskAssessmentAsync(cancellationToken);
        SafetyPlanDTO safetyPlan = await clinicalCareService.GetSafetyPlanAsync(cancellationToken);

        StringBuilder sb = new();
        sb.AppendLine(english ? "Summary for a specialist" : "Сводка для специалиста");
        sb.AppendLine(english
            ? $"Period: last {PeriodDays} days, generated {nowUtc.ToLocalTime():d MMMM yyyy}"
            : $"Период: последние {PeriodDays} дней, сформировано {nowUtc.ToLocalTime():d MMMM yyyy}");
        sb.AppendLine();

        sb.AppendLine(english ? "Practice" : "Практика");
        sb.AppendLine(english
            ? $"- {practiceCount} technique sessions in this period; current streak {streakDays} day(s)."
            : $"- {practiceCount} завершённых практик за период; текущая серия {streakDays} дн. подряд.");
        sb.AppendLine();

        sb.AppendLine(english ? "Mood" : "Настроение");
        if (moods.Count == 0)
        {
            sb.AppendLine(english ? "- No mood check-ins recorded." : "- Записей настроения не было.");
        }
        else
        {
            double avg = moods.Average(entry => entry.MoodLevel);
            sb.AppendLine(english
                ? $"- {moods.Count} check-in(s), average level {avg:F1} of 5 (1 = hard day, 5 = good day)."
                : $"- {moods.Count} записей, средний уровень {avg:F1} из 5 (1 — тяжёлый день, 5 — хороший день).");
        }
        sb.AppendLine();

        sb.AppendLine(english ? "Self-assessment tests" : "Самооценочные тесты");
        if (latestPerTest.Count == 0)
        {
            sb.AppendLine(english ? "- None completed in this period." : "- За этот период тестов не было.");
        }
        else
        {
            foreach (TestResultDTO result in latestPerTest)
            {
                string scoreText = result.Score is int score ? $"{score}" : "—";
                sb.AppendLine(english
                    ? $"- {result.TestId}: {scoreText}, \"{result.Summary}\" ({result.CompletedAt.ToLocalTime():d MMM yyyy})"
                    : $"- {result.TestId}: {scoreText}, «{result.Summary}» ({result.CompletedAt.ToLocalTime():d MMM yyyy})");
            }
        }
        sb.AppendLine();

        sb.AppendLine(english ? "Risk check" : "Проверка безопасности");
        sb.AppendLine(latestRisk is null
            ? english ? "- No risk check on record." : "- Проверок риска не проводилось."
            : english
                ? $"- Last check: {DescribeRisk(latestRisk.RiskLevel, true)} ({latestRisk.AssessedAt.ToLocalTime():d MMM yyyy})."
                : $"- Последняя проверка: {DescribeRisk(latestRisk.RiskLevel, false)} ({latestRisk.AssessedAt.ToLocalTime():d MMM yyyy}).");
        sb.AppendLine();

        sb.AppendLine(english ? "Safety plan" : "План безопасности");
        sb.AppendLine(safetyPlan.IsEmpty
            ? english ? "- Not filled in yet." : "- Ещё не заполнен."
            : english
                ? $"- Filled in: {safetyPlan.WarningSigns.Count} warning sign(s), {safetyPlan.CopingStrategies.Count} coping strateg(y/ies), {safetyPlan.Contacts.Count} contact(s)."
                : $"- Заполнен: признаков — {safetyPlan.WarningSigns.Count}, стратегий — {safetyPlan.CopingStrategies.Count}, контактов — {safetyPlan.Contacts.Count}.");
        sb.AppendLine();

        sb.AppendLine(english
            ? "Generated automatically by the app from the person's own entries. Not a clinical diagnosis."
            : "Сформировано автоматически на основе собственных записей человека в приложении. Не является клиническим диагнозом.");

        return sb.ToString();
    }

    private static string DescribeRisk(RiskLevel level, bool english) => level switch
    {
        RiskLevel.Red => english ? "high (red)" : "высокий (красный)",
        RiskLevel.Amber => english ? "moderate (amber)" : "умеренный (жёлтый)",
        _ => english ? "low (green)" : "низкий (зелёный)"
    };
}
