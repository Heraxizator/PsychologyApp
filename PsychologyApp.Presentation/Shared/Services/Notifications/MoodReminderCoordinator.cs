using PsychologyApp.Domain.Notifications;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class MoodReminderCoordinator(
    IUserPreferencesStore preferencesStore,
    IMoodReminderScheduler scheduler) : IMoodReminderCoordinator
{
    public Task SyncAsync(CancellationToken cancellationToken = default)
    {
        UserPreferencesState state = preferencesStore.Load();

        if (!QuoteReminderPolicy.ShouldSchedule(state.MoodRemindersEnabled, state.HasCompletedOnboarding))
        {
            scheduler.Cancel();
            return Task.CompletedTask;
        }

        return SyncScheduledAsync(state, cancellationToken);
    }

    private async Task SyncScheduledAsync(UserPreferencesState state, CancellationToken cancellationToken)
    {
        await scheduler.RequestPermissionIfNeededAsync(cancellationToken);

        DateTime? nextFireLocal = QuoteReminderPolicy.ResolveNextFireLocal(
            state.MoodRemindersEnabled,
            state.HasCompletedOnboarding,
            state.MoodReminderHour,
            DateTime.Now);

        if (nextFireLocal is null)
        {
            scheduler.Cancel();
            return;
        }

        scheduler.Schedule(
            nextFireLocal.Value,
            AppStrings.MoodReminderTitle,
            AppStrings.MoodReminderBody);
    }
}
