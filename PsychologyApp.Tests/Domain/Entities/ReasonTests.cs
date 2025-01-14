using PsychologyApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsychologyApp.Tests.PsychologyApp.Domain.Tests.Entities;

public class ReasonTests
{
    [Fact]
    public void BuildShouldThrowExceptionIfTittleIsNotSet()
    {
        var action = () => Reason.Create(string.Empty, "Any Subtitle", "Any solution");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfSubtitleIsNotSet()
    {
        var action = () => Reason.Create("Any Title", string.Empty, "Any solution");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldThrowExceptionIfSolutionIsNotSet()
    {
        var action = () => Reason.Create("Any Title", "Any Subtitle", string.Empty);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void BuildShouldCreateIfEveryPropertyIsSet()
    {
        Reason reason = Reason.Create("Any Title", "Any Subtitle", "Any Subject");

        Assert.NotNull(reason);
    }
}
