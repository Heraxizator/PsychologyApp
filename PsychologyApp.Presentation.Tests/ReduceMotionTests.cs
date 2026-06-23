using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ReduceMotionTests
{
    [Fact]
    public void Refresh_ReconfiguresDetectorWithoutThrowing()
    {
        Exception? exception = Record.Exception(ReduceMotion.Refresh);
        Assert.Null(exception);
    }
}
