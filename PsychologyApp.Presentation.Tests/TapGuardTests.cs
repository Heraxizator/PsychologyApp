using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("AsyncErrorHooks")]
public sealed class TapGuardTests
{
    [Fact]
    public void TapDebouncer_AcceptsTheFirstTapAndSwallowsTheNextWithinTheInterval()
    {
        long now = 1_000;
        TapDebouncer debouncer = new(TimeSpan.FromMilliseconds(400), () => now);

        Assert.True(debouncer.TryEnter());
        now += 100;
        Assert.False(debouncer.TryEnter());
        now += 299;
        Assert.False(debouncer.TryEnter());
        now += 1;
        Assert.True(debouncer.TryEnter());
    }

    [Fact]
    public void TapDebouncer_FirstTapIsAcceptedEvenWhenTheClockStartsAtZero()
    {
        TapDebouncer debouncer = new(TimeSpan.FromMilliseconds(400), () => 0);

        Assert.True(debouncer.TryEnter());
    }

    [Fact]
    public void DebouncedCommand_DoubleTapRunsTheStepOnce()
    {
        long now = 5_000;
        int steps = 0;
        DebouncedCommand next = new(() => steps++, debouncer: new TapDebouncer(TimeSpan.FromMilliseconds(400), () => now));

        next.Execute(null);
        next.Execute(null);

        Assert.Equal(1, steps);

        now += 500;
        next.Execute(null);

        Assert.Equal(2, steps);
    }

    [Fact]
    public void DebouncedCommand_WhenItCannotExecute_DoesNotConsumeTheTap()
    {
        long now = 1_000;
        bool allowed = false;
        int steps = 0;
        DebouncedCommand next = new(() => steps++, () => allowed, new TapDebouncer(TimeSpan.FromMilliseconds(400), () => now));

        next.Execute(null);
        allowed = true;
        next.Execute(null);

        Assert.Equal(1, steps);
    }

    [Fact]
    public async Task AsyncCommandOfT_SecondTapWhileRunningIsIgnored()
    {
        TaskCompletionSource release = new();
        int started = 0;
        AsyncCommand<object?> save = new(async _ =>
        {
            Interlocked.Increment(ref started);
            await release.Task;
        });

        save.Execute(3);
        save.Execute(3);

        Assert.Equal(1, started);
        Assert.False(save.CanExecute(3));

        release.SetResult();
        await Task.Delay(50);

        Assert.True(save.CanExecute(3));
        save.Execute(4);
        Assert.Equal(2, started);
    }

    [Fact]
    public async Task AsyncCommandOfT_PassesTheParameterThrough()
    {
        object? received = null;
        TaskCompletionSource done = new();
        AsyncCommand<object?> command = new(parameter =>
        {
            received = parameter;
            done.SetResult();
            return Task.CompletedTask;
        });

        command.Execute("fav");
        await done.Task;

        Assert.Equal("fav", received);
    }

    [Fact]
    public async Task AsyncCommandOfT_AfterAFailure_CanRunAgain()
    {
        Action<Exception>? previous = AsyncCommandExtensions.DefaultErrorHandler;
        List<Exception> errors = [];
        AsyncCommandExtensions.DefaultErrorHandler = errors.Add;
        try
        {
            int calls = 0;
            AsyncCommand<object?> command = new(_ =>
            {
                calls++;
                return calls == 1 ? Task.FromException(new InvalidOperationException("boom")) : Task.CompletedTask;
            });

            command.Execute(null);
            await Task.Delay(50);
            command.Execute(null);
            await Task.Delay(50);

            Assert.Equal(2, calls);
            Assert.Single(errors);
        }
        finally
        {
            AsyncCommandExtensions.DefaultErrorHandler = previous;
        }
    }
}
