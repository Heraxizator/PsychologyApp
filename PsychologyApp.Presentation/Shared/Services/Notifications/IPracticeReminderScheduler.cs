using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public interface IPracticeReminderScheduler
{
    Task RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default);

    void Cancel();

    void Schedule(DateTime fireLocal, TechniqueId techniqueId, string title, string body);
}
