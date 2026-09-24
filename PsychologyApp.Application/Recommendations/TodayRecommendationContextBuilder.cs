using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Recommendations;

public static class TodayRecommendationContextBuilder
{
    private static readonly string[] CatalogTechniqueKeys =
        Enum.GetNames<TechniqueId>();

    /// <summary>A technique needs at least this many completed sessions with both SUDS readings before its
    /// average is trusted enough to steer recommendations.</summary>
    private const int MinSessionsForEffectivenessSignal = 2;

    private const int SessionHistoryLimit = 300;

    public static Task<TodayRecommendationContext> BuildAsync(
        IUserProgressService progress,
        string concern,
        CancellationToken cancellationToken = default) =>
        BuildAsync(progress, concern, clinicalCare: null, cancellationToken);

    public static async Task<TodayRecommendationContext> BuildAsync(
        IUserProgressService progress,
        string concern,
        IClinicalCareService? clinicalCare,
        CancellationToken cancellationToken = default)
    {
        Task<TestResultDTO?> recentTestTask =
            progress.GetMostRecentTestResultAsync(TimeSpan.FromDays(7), cancellationToken);
        Task<IReadOnlyList<MoodEntryDTO>> moodsTask =
            progress.GetRecentMoodsAsync(1, cancellationToken);
        Task<IReadOnlyDictionary<string, DateTime>> datesTask =
            progress.GetLastPracticeDatesAsync(CatalogTechniqueKeys, cancellationToken);
        Task<IReadOnlySet<string>> draftsTask =
            progress.GetSessionDraftKeysAsync(CatalogTechniqueKeys, cancellationToken);
        Task<IReadOnlyList<SessionResultDTO>> sessionResultsTask =
            progress.GetRecentSessionResultsAsync(SessionHistoryLimit, cancellationToken);
        Task<TherapyProgramStateDTO?>? programTask = clinicalCare?.GetActiveProgramAsync(cancellationToken);

        if (programTask is null)
        {
            await Task.WhenAll(recentTestTask, moodsTask, datesTask, draftsTask, sessionResultsTask);
        }
        else
        {
            await Task.WhenAll(recentTestTask, moodsTask, datesTask, draftsTask, sessionResultsTask, programTask);
        }

        int? todayMood = null;
        IReadOnlyList<MoodEntryDTO> moods = await moodsTask;
        if (moods.Count > 0 && moods[0].RecordedAt.ToLocalTime().Date == DateTime.Today)
        {
            todayMood = moods[0].MoodLevel;
        }

        TechniqueId? draftTechniqueId = null;
        foreach (string key in await draftsTask)
        {
            if (Enum.TryParse(key, out TechniqueId techniqueId))
            {
                draftTechniqueId = techniqueId;
                break;
            }
        }

        TherapyProgramStateDTO? program = programTask is null ? null : await programTask;
        IReadOnlyDictionary<string, double> effectiveness = ComputeEffectiveness(await sessionResultsTask);

        return new TodayRecommendationContext(
            concern,
            await recentTestTask,
            todayMood,
            await datesTask,
            draftTechniqueId,
            program?.IsActive == true ? program.ProgramType : null,
            program?.IsActive == true ? program.CurrentWeek : 0,
            effectiveness);
    }

    /// <summary>Average drop in tension (pre minus post SUDS) per technique key, for techniques practiced
    /// often enough that the average means something.</summary>
    private static IReadOnlyDictionary<string, double> ComputeEffectiveness(IReadOnlyList<SessionResultDTO> sessions)
    {
        Dictionary<string, List<int>> deltasByItem = new(StringComparer.Ordinal);
        foreach (SessionResultDTO session in sessions)
        {
            if (session.PreIntensity is not (>= 0 and <= 10) || session.PostIntensity is not (>= 0 and <= 10))
            {
                continue;
            }

            if (!deltasByItem.TryGetValue(session.ItemKey, out List<int>? deltas))
            {
                deltas = [];
                deltasByItem[session.ItemKey] = deltas;
            }

            deltas.Add(session.PreIntensity!.Value - session.PostIntensity!.Value);
        }

        Dictionary<string, double> effectiveness = new(StringComparer.Ordinal);
        foreach ((string itemKey, List<int> deltas) in deltasByItem)
        {
            if (deltas.Count >= MinSessionsForEffectivenessSignal)
            {
                effectiveness[itemKey] = deltas.Average();
            }
        }

        return effectiveness;
    }
}
