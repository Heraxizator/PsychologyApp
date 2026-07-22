using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Recommendations;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.Services.Progress;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class MoodSnapshot
{
    public string TodayMoodDisplay { get; init; } = string.Empty;
    public int SelectedMoodLevel { get; init; }
    public string MoodHistorySummary { get; init; } = string.Empty;
}

public sealed class PracticeDashboardLoader(
    IUserProgressService userProgressService,
    IUserPreferencesStore userPreferencesStore,
    TodayRecommendationResolver todayRecommendationResolver,
    IClinicalCareService clinicalCareService)
{
    private readonly WeeklyInsightLoader _weeklyInsightLoader = new(userProgressService);
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

    public Task<WeeklyInsightSnapshot> LoadWeeklyInsightAsync(CancellationToken cancellationToken = default) =>
        _weeklyInsightLoader.LoadAsync(cancellationToken);

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
            await TodayRecommendationContextBuilder.BuildAsync(
                userProgressService,
                concern,
                clinicalCareService,
                cancellationToken);
        return await todayRecommendationResolver.ResolveAsync(
            context,
            streakDisplay,
            hasStreak,
            navigationService,
            cancellationToken);
    }

    public TechniqueId? ConsumePendingTechnique() => userPreferencesStore.ConsumePendingTechnique();
}

