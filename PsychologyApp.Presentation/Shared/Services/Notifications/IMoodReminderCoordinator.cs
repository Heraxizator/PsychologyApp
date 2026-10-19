namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public interface IMoodReminderCoordinator
{
    Task SyncAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves the next reminder after a check-in, without asking for the notification permission: it runs when the app goes to
    /// the background, where a system dialog would pop up over whatever the person switched to.
    /// </summary>
    Task RescheduleAsync(CancellationToken cancellationToken = default);
}
