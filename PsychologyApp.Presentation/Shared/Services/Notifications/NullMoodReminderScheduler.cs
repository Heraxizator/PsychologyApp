namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class NullMoodReminderScheduler : IMoodReminderScheduler
{
    public bool IsSupported => false;

    public Task<bool> RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public void Schedule(DateTime fireAtLocal, string title, string body)
    {
    }

    public void Cancel()
    {
    }
}
