namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class NullChatReminderScheduler : IChatReminderScheduler
{
    public bool IsSupported => false;

    public Task<bool> RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public void Cancel()
    {
    }

    public void Schedule(DateTime fireAtLocal, string title, string body)
    {
    }
}
