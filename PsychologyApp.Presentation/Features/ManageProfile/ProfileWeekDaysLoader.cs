using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Entities.Journal;
using PsychologyApp.Presentation.Features.ManageJournal;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class ProfileWeekDaysLoader(IUserProgressService userProgressService)
{
    private const int MoodLookbackLimit = 40;

    // Off the UI thread: building the snapshot after each query would otherwise run there.
    public Task<IReadOnlyList<JournalDayChip>> LoadAsync(CancellationToken cancellationToken = default) =>
        Task.Run(() => LoadCoreAsync(cancellationToken), cancellationToken);

    private async Task<IReadOnlyList<JournalDayChip>> LoadCoreAsync(CancellationToken cancellationToken = default)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly rangeStart = today.AddDays(-6);
        DateTime fromUtc = rangeStart.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        DateTime toUtcExclusive = today.AddDays(1).ToDateTime(TimeOnly.MinValue).ToUniversalTime();

        IReadOnlyList<MoodEntryDTO> moods =
            await userProgressService.GetMoodsAsync(fromUtc, toUtcExclusive, MoodLookbackLimit, cancellationToken);

        // The latest check-in of each day, drawn by the same builder as the journal strip.
        Dictionary<DateOnly, int> moodLevelByDay = moods
            .GroupBy(entry => DateOnly.FromDateTime(entry.RecordedAt.ToLocalTime()))
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(entry => entry.RecordedAt).First().MoodLevel);

        return JournalCalendarBuilder.BuildWeekDays(today, moodLevelByDay, selectedDay: null);
    }
}
