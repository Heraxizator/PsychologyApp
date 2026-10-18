using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Notifications;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class MoodReminderCoordinator(
    IUserProgressService progress,
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

        DateTime nowUtc = DateTime.UtcNow;
        IReadOnlyList<MoodEntryDTO> moods = await progress.GetMoodsAsync(
            nowUtc - MoodReminderPolicy.HistoryWindow, nowUtc, 500, cancellationToken);
        List<DateTime> checkInsLocal = [.. moods.Select(mood => mood.RecordedAt.ToLocalTime())];

        int hour = UserPreferences.NormalizeMoodReminderHour(state.MoodReminderHour) == UserPreferences.MoodReminderAutoHour
            ? MoodReminderPolicy.UsualHour(checkInsLocal) ?? UserPreferences.MoodReminderFallbackHour
            : state.MoodReminderHour;

        scheduler.Schedule(
            MoodReminderPolicy.NextFireLocal(hour, checkInsLocal, DateTime.Now),
            AppStrings.MoodReminderTitle,
            AppStrings.MoodReminderBody);
    }
}
