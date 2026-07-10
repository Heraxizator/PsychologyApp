using PsychologyApp.Application.Models;
using PsychologyApp.Application.Quot;
using PsychologyApp.Domain.Notifications;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class QuoteReminderCoordinator(
    IUserPreferencesStore preferencesStore,
    IQuoteReminderScheduler scheduler,
    IQuotService quotService) : IQuoteReminderCoordinator
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

        string body = AppStrings.QuoteReminderBody;
        try
        {
            QuotDTO? daily =
                await quotService.GetDailyQuoteAsync(DateOnly.FromDateTime(DateTime.Today), cancellationToken);
            if (daily is { Text: { Length: > 0 } text })
            {
                body = AppStrings.QuoteReminderBodySnippet(text);
            }
        }
        catch
        {
            // Keep generic body if catalog lookup fails.
        }

        scheduler.Schedule(
            nextFireLocal.Value,
            AppStrings.QuoteReminderTitle,
            body);
    }
}
