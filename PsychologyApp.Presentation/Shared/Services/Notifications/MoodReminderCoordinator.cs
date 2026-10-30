using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Notifications;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class MoodReminderCoordinator(
    IUserProgressService progress,
    IUserPreferencesStore preferencesStore,
    IMoodReminderScheduler scheduler) : IMoodReminderCoordinator
{
    public Task SyncAsync(CancellationToken cancellationToken = default) => SyncAsync(askPermission: true, cancellationToken);

    public Task RescheduleAsync(CancellationToken cancellationToken = default) => SyncAsync(askPermission: false, cancellationToken);

    private Task SyncAsync(bool askPermission, CancellationToken cancellationToken)
    {
        UserPreferencesState state = preferencesStore.Load();

        if (!QuoteReminderPolicy.ShouldSchedule(state.MoodRemindersEnabled, state.HasCompletedOnboarding))
        {
            scheduler.Cancel();
            return Task.CompletedTask;
        }

        return SyncScheduledAsync(state, askPermission, cancellationToken);
    }

    private async Task SyncScheduledAsync(UserPreferencesState state, bool askPermission, CancellationToken cancellationToken)
    {
        if (askPermission)
        {
            if (!await scheduler.RequestPermissionIfNeededAsync(cancellationToken))
            {
                scheduler.Cancel();
                return;
            }
        }

        List<DateTime> checkInsLocal = await LoadCheckInsLocalAsync(cancellationToken);

        int hour = UserPreferences.NormalizeMoodReminderHour(state.MoodReminderHour) == UserPreferences.MoodReminderAutoHour
            ? MoodReminderPolicy.UsualHour(checkInsLocal) ?? UserPreferences.MoodReminderFallbackHour
            : state.MoodReminderHour;

        scheduler.Schedule(
            MoodReminderPolicy.NextFireLocal(hour, checkInsLocal, DateTime.Now),
            AppStrings.MoodReminderTitle,
            AppStrings.MoodReminderBody);
    }

    // The sync also runs from the alarm receiver, where the app may be started without its window. If the check-ins cannot be
    // read there, the reminder still gets its next alarm (at the fallback hour) instead of stopping until the app is opened.
    private async Task<List<DateTime>> LoadCheckInsLocalAsync(CancellationToken cancellationToken)
    {
        try
        {
            DateTime nowUtc = DateTime.UtcNow;
            IReadOnlyList<MoodEntryDTO> moods = await progress.GetMoodsAsync(
                nowUtc - MoodReminderPolicy.HistoryWindow, nowUtc, 500, cancellationToken);
            return [.. moods.Select(mood => mood.RecordedAt.ToLocalTime())];
        }
        catch (Exception readError) when (readError is not OperationCanceledException)
        {
            BestEffort.Report(readError);
            return [];
        }
    }
}
