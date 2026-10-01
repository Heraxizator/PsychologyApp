using System.Diagnostics;

namespace PsychologyApp.Presentation.Shared.Common.Diagnostics;

/// <summary>
/// Debug-only timing for device runs: page/tab creation times and UI-thread stalls, logged under the "PerfTrace"
/// logcat tag. A stall line names the last marks, so a "Skipped N frames" in the log can be tied to its cause.
/// Compiled out of Release.
/// </summary>
public static class PerfTrace
{
    private const int StallThresholdMs = 150;
    private static readonly string?[] RecentMarks = new string?[8];
    private static int _markIndex;
    private static int _started;

    [Conditional("DEBUG")]
    public static void Mark(string what)
    {
        lock (RecentMarks)
        {
            RecentMarks[_markIndex++ % RecentMarks.Length] = $"{DateTime.Now:HH:mm:ss.fff} {what}";
        }
    }

    [Conditional("DEBUG")]
    public static void Measure(string what, long startTimestamp)
    {
        double ms = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
        Mark($"{what} {ms:0} ms");
        Write($"{what}: {ms:0} ms");
    }

    /// <summary>Pings the UI thread from a background thread and reports every reply that came late.</summary>
    [Conditional("DEBUG")]
    public static void StartStallWatchdog(IDispatcher dispatcher)
    {
        if (Interlocked.Exchange(ref _started, 1) == 1)
        {
            return;
        }

        Thread watchdog = new(() =>
        {
            while (true)
            {
                long sent = Stopwatch.GetTimestamp();
                using ManualResetEventSlim replied = new();
                dispatcher.Dispatch(replied.Set);
                replied.Wait();
                double ms = Stopwatch.GetElapsedTime(sent).TotalMilliseconds;
                if (ms >= StallThresholdMs)
                {
                    Write($"UI thread stalled {ms:0} ms; recent: {string.Join(" | ", SnapshotMarks())}");
                }

                Thread.Sleep(100);
            }
        })
        {
            IsBackground = true,
            Name = "PerfTrace watchdog"
        };
        watchdog.Start();
    }

    private static IEnumerable<string> SnapshotMarks()
    {
        lock (RecentMarks)
        {
            List<string> marks = [];
            for (int i = 0; i < RecentMarks.Length; i++)
            {
                if (RecentMarks[(_markIndex + i) % RecentMarks.Length] is { } mark)
                {
                    marks.Add(mark);
                }
            }

            return marks;
        }
    }

    private static void Write(string message)
    {
#if ANDROID
        Android.Util.Log.Info("PerfTrace", message);
#else
        Debug.WriteLine($"PerfTrace: {message}");
#endif
    }
}
