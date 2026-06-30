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

        TechniqueId techniqueId = recommendationService.ResolveFromOnboardingConcern(state.OnboardingConcern);
        scheduler.Schedule(
            nextFireLocal.Value,
            techniqueId,
            AppStrings.PracticeReminderTitle,
            AppStrings.PracticeReminderBody);
    }
}
