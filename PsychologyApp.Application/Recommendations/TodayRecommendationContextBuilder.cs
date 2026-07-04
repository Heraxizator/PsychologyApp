using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;

namespace PsychologyApp.Application.Recommendations;

public static class TodayRecommendationContextBuilder
{
    public static async Task<TodayRecommendationContext> BuildAsync(
        IUserProgressService progress,
        string concern,
        CancellationToken cancellationToken = default)
    {
        TestResultDTO? recentTest = await progress.GetMostRecentTestResultAsync(TimeSpan.FromDays(7), cancellationToken);

        int? todayMood = null;
        IReadOnlyList<MoodEntryDTO> moods = await progress.GetRecentMoodsAsync(1, cancellationToken);
        if (moods.Count > 0 && moods[0].RecordedAt.ToLocalTime().Date == DateTime.Today)
        {
            todayMood = moods[0].MoodLevel;
        }

        return new TodayRecommendationContext(concern, recentTest, todayMood);
    }
}
