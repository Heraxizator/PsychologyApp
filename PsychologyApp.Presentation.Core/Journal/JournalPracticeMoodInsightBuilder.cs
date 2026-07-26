namespace PsychologyApp.Presentation.Features.ManageJournal;

public sealed record JournalPracticeMoodInsightResult(
    bool HasInsight,
    int PracticeDayCount,
    double? AverageMoodOnPracticeDays,
    double? AverageMoodWithoutPractice);

public static class JournalPracticeMoodInsightBuilder
{
    public static JournalPracticeMoodInsightResult Build(
        DateOnly rangeStart,
        DateOnly today,
        IReadOnlySet<DateOnly> practiceDays,
        IReadOnlyDictionary<DateOnly, int> moodLevelByDay)
    {
        HashSet<DateOnly> inRangePracticeDays = practiceDays
            .Where(day => day >= rangeStart && day <= today)
            .ToHashSet();
        if (inRangePracticeDays.Count == 0)
        {
            return new JournalPracticeMoodInsightResult(false, 0, null, null);
        }

        List<int> moodsOnPracticeDays = inRangePracticeDays
            .Where(moodLevelByDay.ContainsKey)
            .Select(day => moodLevelByDay[day])
            .ToList();
        if (moodsOnPracticeDays.Count == 0)
        {
            return new JournalPracticeMoodInsightResult(false, inRangePracticeDays.Count, null, null);
        }

        List<int> moodsWithoutPractice = moodLevelByDay
            .Where(pair => pair.Key >= rangeStart && pair.Key <= today && !inRangePracticeDays.Contains(pair.Key))
            .Select(pair => pair.Value)
            .ToList();

        return new JournalPracticeMoodInsightResult(
            true,
            inRangePracticeDays.Count,
            moodsOnPracticeDays.Average(),
            moodsWithoutPractice.Count > 0 ? moodsWithoutPractice.Average() : null);
    }
}
