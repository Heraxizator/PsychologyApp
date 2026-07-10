using System.Globalization;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Recommendations;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Domain.Tests;
using PsychologyApp.Domain.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class MoodSnapshot
{
    public string TodayMoodDisplay { get; init; } = string.Empty;
    public int SelectedMoodLevel { get; init; }
    public string MoodHistorySummary { get; init; } = string.Empty;
}

public sealed class WeeklyInsightSnapshot
{
    public string DisplayText { get; init; } = string.Empty;
    public bool HasInsight => !string.IsNullOrWhiteSpace(DisplayText);
}

public sealed class PracticeDashboardLoader(
    IUserProgressService userProgressService,
    IUserPreferencesStore userPreferencesStore,
    TodayRecommendationResolver todayRecommendationResolver)
{
    public async Task<int> LoadStreakDaysAsync(CancellationToken cancellationToken = default) =>
        await userProgressService.GetStreakDaysAsync(cancellationToken);

    public async Task<int> LoadAtRiskStreakDaysAsync(CancellationToken cancellationToken = default) =>
        await userProgressService.GetAtRiskStreakDaysAsync(cancellationToken);

    public async Task<DateTime?> LoadLastPracticeUtcAsync(CancellationToken cancellationToken = default) =>
        await userProgressService.GetLastTechniqueCompletionDateAsync(cancellationToken);

    public async Task<string?> LoadLastTechniqueNameAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<CompletionDTO> recent =
            await userProgressService.GetRecentTechniqueCompletionsAsync(1, cancellationToken);
        if (recent.Count == 0)
        {
            return null;
        }

        CompletionDTO last = recent[0];
        if (!string.IsNullOrWhiteSpace(last.PageName))
        {
            return last.PageName.Trim();
        }

        return string.IsNullOrWhiteSpace(last.ItemKey) ? null : last.ItemKey;
    }

    public async Task<bool> HasSessionDraftAsync(TechniqueId techniqueId, CancellationToken cancellationToken = default)
    {
        string? draft = await userProgressService.GetSessionDraftAsync(techniqueId.ToString(), cancellationToken);
        return !string.IsNullOrWhiteSpace(draft);
    }

    public async Task<MoodSnapshot> LoadMoodSnapshotAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MoodEntryDTO> recent = await userProgressService.GetRecentMoodsAsync(3, cancellationToken);
        string todayDisplay = string.Empty;
        int selectedLevel = 0;

        if (recent.Count > 0)
        {
            MoodEntryDTO latest = recent[0];
            DateTime local = latest.RecordedAt.ToLocalTime();
            if (local.Date == DateTime.Today)
            {
                todayDisplay = AppStrings.TodayMoodLine(latest.MoodLevel, 5);
                selectedLevel = latest.MoodLevel;
            }
        }

        IEnumerable<MoodEntryDTO> summarySource = recent.Count > 0 && recent[0].RecordedAt.ToLocalTime().Date == DateTime.Today
            ? recent.Skip(1)
            : recent;

        string[] entries = summarySource
            .Take(2)
            .Select(mood => AppStrings.MoodHistoryEntry(mood.RecordedAt.ToLocalTime().ToString("d"), mood.MoodLevel, 5))
            .ToArray();

        return new MoodSnapshot
        {
            TodayMoodDisplay = todayDisplay,
            SelectedMoodLevel = selectedLevel,
            MoodHistorySummary = entries.Length == 0 ? string.Empty : string.Join(" · ", entries)
        };
    }

    public async Task<WeeklyInsightSnapshot> LoadWeeklyInsightAsync(CancellationToken cancellationToken = default)
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

    public async Task RecordMoodAsync(int moodLevel, CancellationToken cancellationToken = default)
    {
        await userProgressService.RecordMoodAsync(moodLevel, cancellationToken: cancellationToken);
    }

    public async Task<TodayRecommendationResult> ResolveTodayRecommendationAsync(
        int streakDays,
        INavigationService navigationService,
        CancellationToken cancellationToken = default)
    {
        string streakDisplay = AppStrings.ProfileStreakCount(streakDays);
        bool hasStreak = streakDays > 0;
        string concern = userPreferencesStore.Load().OnboardingConcern;
        TodayRecommendationContext context =
            await TodayRecommendationContextBuilder.BuildAsync(userProgressService, concern, cancellationToken);
        return await todayRecommendationResolver.ResolveAsync(
            context,
            streakDisplay,
            hasStreak,
            navigationService,
            cancellationToken);
    }

    public TechniqueId? ConsumePendingTechnique() => userPreferencesStore.ConsumePendingTechnique();

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
