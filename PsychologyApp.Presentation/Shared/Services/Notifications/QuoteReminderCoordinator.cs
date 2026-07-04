using PsychologyApp.Domain.Notifications;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class QuoteReminderCoordinator(
    IUserPreferencesStore preferencesStore,
    IQuoteReminderScheduler scheduler) : IQuoteReminderCoordinator
{
    public Task SyncAsync(CancellationToken cancellationToken = default)
    {
        UserPreferencesState state = preferencesStore.Load();

        if (!QuoteReminderPolicy.ShouldSchedule(state.QuoteRemindersEnabled, state.HasCompletedOnboarding))
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
            state.QuoteRemindersEnabled,
            state.HasCompletedOnboarding,
            state.QuoteReminderHour,
            DateTime.Now);

        if (nextFireLocal is null)
        {
            scheduler.Cancel();
            return;
        }

        scheduler.Schedule(
            nextFireLocal.Value,
            AppStrings.QuoteReminderTitle,
            AppStrings.QuoteReminderBody);
    }
}
