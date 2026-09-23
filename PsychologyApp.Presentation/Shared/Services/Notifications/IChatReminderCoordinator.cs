namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public interface IChatReminderCoordinator
{
    Task SyncAsync(CancellationToken cancellationToken = default);
}
