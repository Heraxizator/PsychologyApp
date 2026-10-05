using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Shared.Common;

public static class AsyncCommandExtensions
{
    public static Action<Exception>? DefaultErrorHandler { get; set; }

    /// <summary>Receives the failure and where it was started from; wired to the logger at startup so every background failure is traceable.</summary>
    public static Action<Exception, string>? DefaultErrorLogger { get; set; }

    public static void FireAndForget(
        this Task task,
        Action<Exception>? onError = null,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "")
    {
        _ = task.ContinueWith(
            t =>
            {
                if (t.IsCanceled)
                {
                    return;
                }

                if (t.Exception is null)
                {
                    return;
                }

                Exception error = t.Exception.GetBaseException();
                if (error is OperationCanceledException)
                {
                    return;
                }

                string origin = $"{Path.GetFileNameWithoutExtension(file)}.{caller}";
                if (DefaultErrorLogger is { } log)
                {
                    log(error, origin);
                }
                else
                {
                    System.Diagnostics.Trace.TraceError($"Unobserved async failure in {origin}: {error}");
                }

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
