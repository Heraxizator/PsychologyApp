using PsychologyApp.Application.Reason;
using Xunit;

namespace PsychologyApp.Application.Tests.Reason;

public sealed class ReasonTsvParserTests
{
    [Fact]
    public void ParseLines_SkipsInvalidRows()
    {
        IReadOnlyList<PsychologyApp.Domain.Entities.Reason> reasons = ReasonTsvParser.ParseLines(
        [
            "headache\tstress\trest",
            "incomplete",
            "\t\t",
            "pain\tcause\tsolution"
        ]);

        Assert.Equal(2, reasons.Count);
        Assert.Equal("headache", reasons[0].Title);
        Assert.Equal("pain", reasons[1].Title);
    }
}
