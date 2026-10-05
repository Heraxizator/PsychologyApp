using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("AsyncErrorHooks")]
public sealed class SafeAsyncTests
{
    private static async Task<T> Within<T>(TaskCompletionSource<T> source) => await source.Task.WaitAsync(TimeSpan.FromSeconds(5));

    [Fact]
    public async Task AFailureAfterTheFirstAwait_IsReportedWithWhereItStarted_AndDoesNotEscape()
    {
        TaskCompletionSource<(Exception Error, string Origin)> seen = new();
        Action<Exception, string>? previous = AsyncCommandExtensions.DefaultErrorLogger;
        AsyncCommandExtensions.DefaultErrorLogger = (e, o) => seen.TrySetResult((e, o));
        try
        {
            SafeAsync.Run(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("boom");
            });

            (Exception error, string origin) = await Within(seen);
            Assert.IsType<InvalidOperationException>(error);
            Assert.Contains(nameof(AFailureAfterTheFirstAwait_IsReportedWithWhereItStarted_AndDoesNotEscape), origin);
            Assert.Contains(nameof(SafeAsyncTests), origin);
        }
        finally
        {
            AsyncCommandExtensions.DefaultErrorLogger = previous;
        }
    }

    [Fact]
    public async Task AFailureBeforeTheFirstAwait_IsReportedToo()
    {
        TaskCompletionSource<Exception> seen = new();
        Action<Exception, string>? previous = AsyncCommandExtensions.DefaultErrorLogger;
        AsyncCommandExtensions.DefaultErrorLogger = (e, _) => seen.TrySetResult(e);
        try
        {
            SafeAsync.Run(() => throw new InvalidOperationException("sync"));

            Assert.Equal("sync", (await Within(seen)).Message);
        }
        finally
        {
            AsyncCommandExtensions.DefaultErrorLogger = previous;
        }
    }

    [Fact]
    public async Task ACancellation_IsNotAFailure()
    {
        bool logged = false;
        bool handled = false;
        Action<Exception, string>? previous = AsyncCommandExtensions.DefaultErrorLogger;
        AsyncCommandExtensions.DefaultErrorLogger = (_, _) => logged = true;
        try
        {
            using CancellationTokenSource cts = new();
            await cts.CancelAsync();
            Task cancelled = Task.Delay(Timeout.Infinite, cts.Token);

            cancelled.FireAndForget(_ => handled = true);
            await Task.Delay(100);

            Assert.False(logged);
            Assert.False(handled);
        }
        finally
        {
            AsyncCommandExtensions.DefaultErrorLogger = previous;
        }
    }
}
