using Xunit;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Tests.ViewModels;

internal static class VmTestHelpers
{
    /// <summary>Runs a command and waits until it has finished (the app's async commands are fire-and-forget by design).</summary>
    public static async Task RunAsync(ICommand command)
    {
        command.Execute(null);
        await WaitUntilAsync(() => command.CanExecute(null));
    }

    public static async Task WaitUntilAsync(Func<bool> condition, int timeoutMs = 3000)
    {
        DateTime until = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        while (!condition())
        {
            if (DateTime.UtcNow > until)
            {
                throw new TimeoutException("The condition was not met in time.");
            }

            await Task.Delay(10);
        }
    }
}

[CollectionDefinition("StaticShims", DisableParallelization = true)]
public sealed class StaticShimsCollection;
