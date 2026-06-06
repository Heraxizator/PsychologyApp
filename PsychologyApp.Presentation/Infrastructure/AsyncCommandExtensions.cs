namespace PsychologyApp.Presentation.Infrastructure;

public static class AsyncCommandExtensions
{
    public static Action<Exception>? DefaultErrorHandler { get; set; }

    public static void FireAndForget(this Task task, Action<Exception>? onError = null)
    {
        _ = task.ContinueWith(
            t =>
            {
                if (t.Exception is null)
                {
                    return;
                }

                Exception error = t.Exception.GetBaseException();
                if (onError is not null)
                {
                    onError.Invoke(error);
                    return;
                }

                DefaultErrorHandler?.Invoke(error);
            },
            TaskScheduler.Default);
    }
}
