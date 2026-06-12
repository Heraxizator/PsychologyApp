using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class AsyncCommandTests
{
    [Fact]
    public async Task Execute_RunsDelegateOnce()
    {
        int executions = 0;
        var command = new AsyncCommand(async () =>
        {
            executions++;
            await Task.CompletedTask;
        });

        command.Execute(null);
        await Task.Delay(50);

        Assert.Equal(1, executions);
    }

    [Fact]
    public void CanExecute_ReturnsFalseWhileRunning()
    {
        var gate = new TaskCompletionSource();
        var command = new AsyncCommand(async () => await gate.Task);

        command.Execute(null);

        Assert.False(command.CanExecute(null));
    }

    [Fact]
    public async Task Execute_IgnoresConcurrentInvocations()
    {
        int executions = 0;
        var gate = new TaskCompletionSource();
        var command = new AsyncCommand(async () =>
        {
            executions++;
            await gate.Task;
        });

        command.Execute(null);
        command.Execute(null);
        gate.SetResult();
        await Task.Delay(50);

        Assert.Equal(1, executions);
    }

}
