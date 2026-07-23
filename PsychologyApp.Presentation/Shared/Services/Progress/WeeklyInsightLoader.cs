using System.Globalization;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Tests;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.Services.Progress;

public sealed class WeeklyInsightSnapshot
{
    public string DisplayText { get; init; } = string.Empty;
    public int PracticeCount { get; init; }
    public int MoodEntriesCount { get; init; }
    public string MoodTrendLabel { get; init; } = string.Empty;
    public int StreakDays { get; init; }
    public string ExtraPillText { get; init; } = string.Empty;
    public string WeekRangeLabel { get; init; } = string.Empty;

    public bool HasInsight => PracticeCount > 0 || MoodEntriesCount > 0;
    public bool HasMoodTrend => !string.IsNullOrWhiteSpace(MoodTrendLabel);
    public bool HasExtraPill => !string.IsNullOrWhiteSpace(ExtraPillText);
}

public sealed class WeeklyInsightLoader(IUserProgressService userProgressService)
{
    public async Task<WeeklyInsightSnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly weekStart = GetWeekStart(today);
        DateTime weekStartLocal = weekStart.ToDateTime(TimeOnly.MinValue);
        string weekRange = AppStrings.WeekRangeLabel(weekStart, today);

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

        int streakDays = await streakTask;
        string testPill = await ResolveTestPillAsync(
            await recentTestTask,
            weekStartLocal,
            cancellationToken);

        if (practiceCount == 0 && weekMoods.Count == 0)
        {
            return new WeeklyInsightSnapshot
            {
                WeekRangeLabel = weekRange,
                StreakDays = streakDays,
                ExtraPillText = testPill
            };
        }

        string moodTrend = ResolveMoodTrend(weekMoods);
        string baseLine = practiceCount == 0
            ? AppStrings.WeeklyInsightMoodOnly(moodTrend)
            : AppStrings.WeeklyInsightLine(practiceCount, moodTrend);

        string streakPart = streakDays > 0
            ? AppStrings.WeeklyInsightStreakPart(streakDays)
            : string.Empty;
        string extraForDisplay = !string.IsNullOrWhiteSpace(streakPart) ? streakPart : testPill;

        return new WeeklyInsightSnapshot
        {
            DisplayText = AppStrings.WeeklyInsightWithExtra(baseLine, extraForDisplay),
            PracticeCount = practiceCount,
            MoodEntriesCount = weekMoods.Count,
            MoodTrendLabel = moodTrend,
            StreakDays = streakDays,
            ExtraPillText = testPill,
            WeekRangeLabel = weekRange
        };
    }

    private async Task<string> ResolveTestPillAsync(
        TestResultDTO? recentTest,
        DateTime weekStartLocal,
        CancellationToken cancellationToken)
    {
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
