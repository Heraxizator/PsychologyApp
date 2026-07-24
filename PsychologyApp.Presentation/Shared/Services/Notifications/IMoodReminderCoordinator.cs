namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public interface IMoodReminderCoordinator
{
    Task SyncAsync(CancellationToken cancellationToken = default);
}
