using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// The one way to start async work from a place that cannot await (event handlers, lifecycle overrides).
/// An exception escaping an <c>async void</c> method ends the process; here it is reported like any other background failure.
/// </summary>
public static class SafeAsync
{
    public static void Run(
        Func<Task> work,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "")
    {
        Task task;
        try
        {
            task = work();
        }
        catch (Exception ex)
        {
            // The synchronous part of the work threw before the first await.
            Task.FromException(ex).FireAndForget(null, caller, file);
            return;
        }

        task.FireAndForget(null, caller, file);
    }
}
