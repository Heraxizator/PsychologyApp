using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// For work whose failure must not stop the screen (an optional trend, a dial intent) but must not vanish either:
/// the failure is logged with where it happened and the person is not interrupted.
/// </summary>
public static class BestEffort
{
    public static void Report(
        Exception error,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "")
    {
        if (error is OperationCanceledException)
        {
            return;
        }

        string origin = $"{Path.GetFileNameWithoutExtension(file)}.{caller}";
        if (AsyncCommandExtensions.DefaultErrorLogger is { } log)
        {
            log(error, origin);
        }
        else
        {
            System.Diagnostics.Trace.TraceWarning($"Ignored failure in {origin}: {error.Message}");
        }
    }
}
