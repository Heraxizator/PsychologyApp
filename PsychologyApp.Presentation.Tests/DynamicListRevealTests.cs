using PsychologyApp.Presentation.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class DynamicListRevealTests
{
    [Fact]
    public void Attach_RevealsNewChildWithFadeOnly()
    {
        VerticalStackLayout layout = new();
        DynamicListReveal.Attach(layout);

        Label label = new();
        layout.Add(label);

        Assert.Equal(1, label.Opacity);
    }
}
