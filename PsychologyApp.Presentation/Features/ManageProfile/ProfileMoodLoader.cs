using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed record ProfileMoodSnapshot(
    IReadOnlyList<MoodChartPoint> ChartPoints,
    bool HasTrendChart);

public sealed class ProfileMoodLoader(IUserProgressService userProgressService)
{
    public async Task<ProfileMoodSnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MoodEntryDTO> moods = await userProgressService.GetRecentMoodsAsync(30, cancellationToken);
        List<MoodChartPoint> points = moods
            .OrderBy(entry => entry.RecordedAt)
            .Select(entry => new MoodChartPoint(entry.RecordedAt.ToLocalTime(), entry.MoodLevel))
            .ToList();

        return new ProfileMoodSnapshot(points, points.Count >= 3);
    }
}
