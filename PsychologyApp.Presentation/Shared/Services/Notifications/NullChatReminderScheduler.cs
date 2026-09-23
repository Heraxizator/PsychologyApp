namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class NullChatReminderScheduler : IChatReminderScheduler
{
    public bool IsSupported => false;

    public Task RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public void Cancel()
    {
    }

    public void Schedule(DateTime fireAtLocal, string title, string body)
    {
    }
}
