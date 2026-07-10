namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class NullQuoteReminderScheduler : IQuoteReminderScheduler
{
    public bool IsSupported => false;

    public Task RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public void Schedule(DateTime fireAtLocal, string title, string body)
    {
    }

    public void Cancel()
    {
    }
}
