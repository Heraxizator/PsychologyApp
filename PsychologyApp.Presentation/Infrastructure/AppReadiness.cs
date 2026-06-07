namespace PsychologyApp.Presentation.Infrastructure;

public static class AppReadiness
{
    private static readonly TaskCompletionSource _databaseReady = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public static bool IsDatabaseReady { get; private set; }

    public static Task DatabaseReadyAsync => _databaseReady.Task;

    public static void SignalDatabaseReady()
    {
        IsDatabaseReady = true;
        _databaseReady.TrySetResult();
    }

    public static void SignalDatabaseFailed(Exception exception) =>
        _databaseReady.TrySetException(exception);
}
