using System.Globalization;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Entities.Journal;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class ProfileWeekDaysLoader(IUserProgressService userProgressService)
{
    private const int MoodLookbackLimit = 40;

    public async Task<IReadOnlyList<JournalDayChip>> LoadAsync(CancellationToken cancellationToken = default)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly rangeStart = today.AddDays(-6);
        DateTime fromUtc = rangeStart.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        DateTime toUtcExclusive = today.AddDays(1).ToDateTime(TimeOnly.MinValue).ToUniversalTime();

        IReadOnlyList<MoodEntryDTO> moods =
            await userProgressService.GetMoodsAsync(fromUtc, toUtcExclusive, MoodLookbackLimit, cancellationToken);

        Dictionary<DateOnly, MoodEntryDTO> byDay = moods
            .GroupBy(entry => DateOnly.FromDateTime(entry.RecordedAt.ToLocalTime()))
            .ToDictionary(
                group => group.Key,
                group => group.OrderByDescending(entry => entry.RecordedAt).First());

        List<JournalDayChip> chips = [];
        for (int offset = 6; offset >= 0; offset--)
        {
            DateOnly date = today.AddDays(-offset);
            byDay.TryGetValue(date, out MoodEntryDTO? entry);
            chips.Add(new JournalDayChip
            {
                Date = date,
                DayLabel = date.ToDateTime(TimeOnly.MinValue)
                    .ToString("ddd", CultureInfo.CurrentCulture),
                MoodGlyph = entry is null ? "·" : AppStrings.MoodEmojiFor(entry.MoodLevel),
                MoodLevel = entry?.MoodLevel,
                HasEntry = entry is not null,
                IsSelected = false
            });
        }

        return chips;
    }
}
