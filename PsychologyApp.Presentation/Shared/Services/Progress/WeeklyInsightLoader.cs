using System.Globalization;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Tests;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.Services.Progress;

public sealed class WeeklyInsightSnapshot
{
    public string DisplayText { get; init; } = string.Empty;

    public bool HasInsight => !string.IsNullOrWhiteSpace(DisplayText);
}

public sealed class WeeklyInsightLoader(IUserProgressService userProgressService)
{
    public async Task<WeeklyInsightSnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        DateOnly weekStart = GetWeekStart(DateOnly.FromDateTime(DateTime.Today));
        DateTime weekStartLocal = weekStart.ToDateTime(TimeOnly.MinValue);

        Task<IReadOnlyList<CompletionDTO>> completionsTask =
            userProgressService.GetRecentTechniqueCompletionsAsync(50, cancellationToken);
        Task<IReadOnlyList<MoodEntryDTO>> moodsTask =
            userProgressService.GetRecentMoodsAsync(30, cancellationToken);
        Task<int> streakTask = userProgressService.GetStreakDaysAsync(cancellationToken);
        Task<TestResultDTO?> recentTestTask =
            userProgressService.GetMostRecentTestResultAsync(TimeSpan.FromDays(7), cancellationToken);
        await Task.WhenAll(completionsTask, moodsTask, streakTask, recentTestTask);

        int practiceCount = (await completionsTask)
            .Count(completion => completion.CompletedAt.ToLocalTime() >= weekStartLocal);

        List<MoodEntryDTO> weekMoods = (await moodsTask)
            .Where(mood => mood.RecordedAt.ToLocalTime() >= weekStartLocal)
            .OrderBy(mood => mood.RecordedAt)
            .ToList();

        if (practiceCount == 0 && weekMoods.Count == 0)
        {
            return new WeeklyInsightSnapshot();
        }

        string moodTrend = ResolveMoodTrend(weekMoods);
        string baseLine = practiceCount == 0
            ? AppStrings.WeeklyInsightMoodOnly(moodTrend)
            : AppStrings.WeeklyInsightLine(practiceCount, moodTrend);

        string extra = await ResolveWeeklyExtraAsync(
            await streakTask,
            await recentTestTask,
            weekStartLocal,
            cancellationToken);

        return new WeeklyInsightSnapshot
        {
            DisplayText = AppStrings.WeeklyInsightWithExtra(baseLine, extra)
        };
    }

    private async Task<string> ResolveWeeklyExtraAsync(
        int streakDays,
        TestResultDTO? recentTest,
        DateTime weekStartLocal,
        CancellationToken cancellationToken)
    {
        if (streakDays > 0)
        {
            return AppStrings.WeeklyInsightStreakPart(streakDays);
        }

        if (recentTest is null || string.IsNullOrWhiteSpace(recentTest.TestId))
        {
            return string.Empty;
        }

        IReadOnlyList<TestResultDTO> history =
            await userProgressService.GetTestResultHistoryAsync(recentTest.TestId, 2, cancellationToken);
        if (history.Count < 2)
        {
            return string.Empty;
        }

        bool bothThisWeek = history.Take(2).All(item => item.CompletedAt.ToLocalTime() >= weekStartLocal);
        if (!bothThisWeek)
        {
            return string.Empty;
        }

        TestTrendKind kind = TestTrendEvaluator.CompareScores(history[0].Score, history[1].Score);
        return kind switch
        {
            TestTrendKind.Improved => AppStrings.WeeklyInsightTestImprovedPart(),
            TestTrendKind.Worse => AppStrings.WeeklyInsightTestWorsePart(),
            _ => string.Empty
        };
    }

    private static DateOnly GetWeekStart(DateOnly today)
    {
        DayOfWeek firstDay = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        int offset = (7 + (today.DayOfWeek - firstDay)) % 7;
        return today.AddDays(-offset);
    }

    private static string ResolveMoodTrend(IReadOnlyList<MoodEntryDTO> weekMoods)
    {
        if (weekMoods.Count == 0)
        {
            return string.Empty;
        }

        if (weekMoods.Count == 1)
        {
            return AppStrings.MoodTrendFlat;
        }

        int first = weekMoods[0].MoodLevel;
        int last = weekMoods[^1].MoodLevel;
        if (last > first)
        {
            return AppStrings.MoodTrendUp;
        }

        if (last < first)
        {
            return AppStrings.MoodTrendDown;
        }

        return AppStrings.MoodTrendFlat;
    }
}
