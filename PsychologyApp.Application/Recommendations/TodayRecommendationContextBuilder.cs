using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Recommendations;

public static class TodayRecommendationContextBuilder
{
    private static readonly string[] CatalogTechniqueKeys =
        Enum.GetNames<TechniqueId>();

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
        Task<TherapyProgramStateDTO?>? programTask = clinicalCare?.GetActiveProgramAsync(cancellationToken);

        if (programTask is null)
        {
            await Task.WhenAll(recentTestTask, moodsTask, datesTask, draftsTask);
        }
        else
        {
            await Task.WhenAll(recentTestTask, moodsTask, datesTask, draftsTask, programTask);
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

        return new TodayRecommendationContext(
            concern,
            await recentTestTask,
            todayMood,
            await datesTask,
            draftTechniqueId,
            program?.IsActive == true ? program.ProgramType : null,
            program?.IsActive == true ? program.CurrentWeek : 0);
    }
}
