using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed record ProfileMoodSnapshot(
    IReadOnlyList<MoodChartPoint> ChartPoints,
    bool HasTrendChart,
    string ChartSubtitle,
    IReadOnlyList<MoodNoteItem> RecentNotes);

public sealed class ProfileMoodLoader(IUserProgressService userProgressService)
{
    private const int RecentNotesLimit = 3;

    public async Task<ProfileMoodSnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MoodEntryDTO> moods = await userProgressService.GetRecentMoodsAsync(30, cancellationToken);
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

        return new ProfileMoodSnapshot(
            points,
            points.Count >= 2,
            AppStrings.ResolveChartSubtitle(points.Count),
            notes);
    }
}
