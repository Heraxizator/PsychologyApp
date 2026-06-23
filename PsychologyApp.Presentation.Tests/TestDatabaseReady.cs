using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Tests;

internal static class TestDatabaseReady
{
    public static DatabaseReadySignal CreateSignaled()
    {
        DatabaseReadySignal signal = new();
        signal.SignalReady();
        return signal;
    }
}
