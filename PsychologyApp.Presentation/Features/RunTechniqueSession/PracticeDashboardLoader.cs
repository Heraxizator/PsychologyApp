using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
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

public sealed class PracticeDashboardLoader(
    IUserProgressService userProgressService,
    IUserPreferencesStore userPreferencesStore)
{
    public async Task<int> LoadStreakDaysAsync(CancellationToken cancellationToken = default) =>
        await userProgressService.GetStreakDaysAsync(cancellationToken);

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

    public async Task RecordMoodAsync(int moodLevel, CancellationToken cancellationToken = default)
    {
        await userProgressService.RecordMoodAsync(moodLevel, cancellationToken: cancellationToken);
    }

    public TodayRecommendationResult ResolveTodayRecommendation(
        int streakDays,
        INavigationService navigationService)
    {
        string streakDisplay = AppStrings.ProfileStreakCount(streakDays);
        bool hasStreak = streakDays > 0;
        return TodayRecommendationResolver.Resolve(
            userPreferencesStore.Load().OnboardingConcern,
            streakDisplay,
            hasStreak,
            navigationService);
    }

    public TechniqueId? ConsumePendingTechnique() => userPreferencesStore.ConsumePendingTechnique();
}
