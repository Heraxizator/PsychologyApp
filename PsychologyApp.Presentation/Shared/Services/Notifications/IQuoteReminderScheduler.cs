namespace PsychologyApp.Presentation.Shared.Services.Notifications;

public interface IQuoteReminderScheduler
{
    bool IsSupported { get; }

    Task RequestPermissionIfNeededAsync(CancellationToken cancellationToken = default);

    void Schedule(DateTime fireAtLocal, string title, string body);

    void Cancel();
}
