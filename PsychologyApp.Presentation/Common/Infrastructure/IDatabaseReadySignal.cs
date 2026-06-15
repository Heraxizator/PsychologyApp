namespace PsychologyApp.Presentation.Common.Infrastructure;

public interface IDatabaseReadySignal
{
    bool IsReady { get; }

    Task WaitAsync(CancellationToken cancellationToken = default);

    void SignalReady();

    void SignalFailed(Exception exception);
}

public sealed class DatabaseReadySignal : IDatabaseReadySignal
{
    private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public bool IsReady { get; private set; }

    public Task WaitAsync(CancellationToken cancellationToken = default) =>
        _ready.Task.WaitAsync(cancellationToken);

    public void SignalReady()
    {
        IsReady = true;
        _ready.TrySetResult();
    }

    public void SignalFailed(Exception exception) =>
        _ready.TrySetException(exception);
}
