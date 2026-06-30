namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public interface IPracticeReminderCoordinator
{
    Task SyncAsync(CancellationToken cancellationToken = default);
}
