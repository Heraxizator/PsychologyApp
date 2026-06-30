using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public sealed class NullPracticeReminderScheduler : IPracticeReminderScheduler
{
    public Task RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public void Cancel()
    {
    }

    public void Schedule(DateTime fireLocal, TechniqueId techniqueId, string title, string body)
    {
    }
}
