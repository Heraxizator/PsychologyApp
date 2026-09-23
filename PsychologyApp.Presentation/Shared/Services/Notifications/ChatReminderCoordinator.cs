using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Chat;
using PsychologyApp.Domain.Notifications;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class ChatReminderCoordinator(
    IChatRepository chatRepository,
    IUserPreferencesStore preferencesStore,
    IChatReminderScheduler scheduler) : IChatReminderCoordinator
{
    public async Task SyncAsync(CancellationToken cancellationToken = default)
    {
        UserPreferencesState state = preferencesStore.Load();

        if (!ChatReminderPolicy.ShouldSchedule(state.ChatRemindersEnabled, state.HasCompletedOnboarding))
        {
            scheduler.Cancel();
            return;
        }

        await scheduler.RequestPermissionIfNeededAsync(cancellationToken);

        DateTime? lastActivityUtc = await GetLastChatActivityUtcAsync(cancellationToken);
        DateTime? nextFireLocal = ChatReminderPolicy.ResolveNextFireLocal(
            state.ChatRemindersEnabled,
            state.HasCompletedOnboarding,
            lastActivityUtc,
            state.ChatReminderHour,
            DateTime.Now);

        if (nextFireLocal is null)
        {
            scheduler.Cancel();
            return;
        }

        scheduler.Schedule(nextFireLocal.Value, AppStrings.ChatReminderTitle, AppStrings.ChatReminderBody);
    }

    private async Task<DateTime?> GetLastChatActivityUtcAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<ChatSessionDTO> sessions = await chatRepository.GetSessionsAsync(cancellationToken);
        return sessions.Count == 0 ? null : sessions[0].UpdatedAt;
    }
}
