using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Progress;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed record ProfileMoodSnapshot(
    IReadOnlyList<MoodChartPoint> ChartPoints,
    bool HasTrendChart,
    string ChartSubtitle,
    IReadOnlyList<MoodNoteItem> RecentNotes,
    int SelectedMoodLevel,
    string TodayMoodDisplay,
    string MoodHistorySummary,
    string WeeklyInsightText);

public sealed class ProfileMoodLoader(
    IUserProgressService userProgressService,
    WeeklyInsightLoader weeklyInsightLoader)
{
    private const int RecentNotesLimit = 3;

    public async Task<ProfileMoodSnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        Task<IReadOnlyList<MoodEntryDTO>> moodsTask =
            userProgressService.GetRecentMoodsAsync(30, cancellationToken);
        Task<WeeklyInsightSnapshot> insightTask = weeklyInsightLoader.LoadAsync(cancellationToken);
        await Task.WhenAll(moodsTask, insightTask);

        IReadOnlyList<MoodEntryDTO> moods = await moodsTask;
        List<MoodChartPoint> points = moods
            .OrderBy(entry => entry.RecordedAt)
            .Select(entry => new MoodChartPoint(entry.RecordedAt.ToLocalTime(), entry.MoodLevel))
            .ToList();

        List<MoodNoteItem> notes = moods
            .Where(entry => !string.IsNullOrWhiteSpace(entry.Note))
            .Take(RecentNotesLimit)
            .Select(entry => new MoodNoteItem(
                entry.RecordedAt.ToLocalTime().ToString("d"),
                entry.Note!.Trim()))
            .ToList();

        string todayDisplay = string.Empty;
        int selectedLevel = 0;
        if (moods.Count > 0)
        {
            MoodEntryDTO latest = moods[0];
            if (latest.RecordedAt.ToLocalTime().Date == DateTime.Today)
            {
                todayDisplay = AppStrings.TodayMoodLine(latest.MoodLevel, 5);
                selectedLevel = latest.MoodLevel;
            }
        }

        IEnumerable<MoodEntryDTO> summarySource = moods.Count > 0 && moods[0].RecordedAt.ToLocalTime().Date == DateTime.Today
            ? moods.Skip(1)
            : moods;
        string[] historyEntries = summarySource
            .Take(2)
            .Select(mood => AppStrings.MoodHistoryEntry(mood.RecordedAt.ToLocalTime().ToString("d"), mood.MoodLevel, 5))
            .ToArray();

        return new ProfileMoodSnapshot(
            points,
            points.Count >= 2,
            AppStrings.ResolveChartSubtitle(points.Count),
            notes,
            selectedLevel,
            todayDisplay,
            historyEntries.Length == 0 ? string.Empty : string.Join(" · ", historyEntries),
            (await insightTask).DisplayText);
    }

    public Task RecordMoodAsync(int moodLevel, string? note = null, CancellationToken cancellationToken = default) =>
        userProgressService.RecordMoodAsync(moodLevel, note, cancellationToken);
}
