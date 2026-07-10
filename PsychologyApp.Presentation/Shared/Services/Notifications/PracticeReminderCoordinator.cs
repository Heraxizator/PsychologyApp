using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.Recommendations;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Notifications;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class PracticeReminderCoordinator(
    IUserProgressService progress,
    IUserPreferencesStore preferencesStore,
    ITechniqueRecommendationService recommendationService,
    ITechniqueCatalogService techniqueCatalog,
    IPracticeReminderScheduler scheduler) : IPracticeReminderCoordinator
{
    public async Task SyncAsync(CancellationToken cancellationToken = default)
    {
        UserPreferencesState state = preferencesStore.Load();

        if (!PracticeReminderPolicy.ShouldSchedule(state.PracticeRemindersEnabled, state.HasCompletedOnboarding))
        {
            scheduler.Cancel();
            return;
        }

        await scheduler.RequestPermissionIfNeededAsync(cancellationToken);

        DateTime? lastPracticeUtc = await progress.GetLastTechniqueCompletionDateAsync(cancellationToken);
        DateTime nowLocal = DateTime.Now;
        DateTime? nextFireLocal = PracticeReminderPolicy.ResolveNextFireLocal(
            state.PracticeRemindersEnabled,
            state.HasCompletedOnboarding,
            lastPracticeUtc,
            state.PracticeReminderHour,
            nowLocal);

        if (nextFireLocal is null)
        {
            scheduler.Cancel();
            return;
        }

        TodayRecommendationContext context =
            await TodayRecommendationContextBuilder.BuildAsync(progress, state.OnboardingConcern, cancellationToken);
        TodayRecommendationDecision decision = recommendationService.ResolveTodayTechnique(context);
        TechniqueId techniqueId = decision.TechniqueId;
        BuiltInTechniqueDefinition definition = await techniqueCatalog.GetAsync(techniqueId, cancellationToken);
        string reason = ResolveReasonText(decision, context);

        scheduler.Schedule(
            nextFireLocal.Value,
            techniqueId,
            AppStrings.PracticeReminderTitleNamed(definition.ListTitle),
            AppStrings.PracticeReminderBodyNamed(definition.ListTitle, reason));
    }

    private static string ResolveReasonText(TodayRecommendationDecision decision, TodayRecommendationContext context) =>
        decision.Source switch
        {
            TodayRecommendationSource.RecentTest =>
                AppStrings.TodayRecommendationReasonFromTest(decision.TestId ?? context.RecentTestResult?.TestId ?? string.Empty),
            TodayRecommendationSource.LowMood => AppStrings.TodayRecommendationReasonLowMood(),
            _ => AppStrings.TodayRecommendationReason(context.Concern)
        };
}
