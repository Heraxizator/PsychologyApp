using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class NullPracticeReminderScheduler : IPracticeReminderScheduler
{
    public bool IsSupported => false;

    public Task<bool> RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(false);

    public void Cancel()
    {
    }

    public void Schedule(DateTime fireLocal, TechniqueId techniqueId, string title, string body)
    {
    }
}
