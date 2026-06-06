using DomainStatistic = PsychologyApp.Domain.Entities.Statistic;
using Xunit;

namespace PsychologyApp.Domain.Tests.Entities;

public class StatisticTests
{
    [Fact]
    public void Create_WithValidData_Succeeds()
    {
        var statistic = DomainStatistic.Create("Module", "Page", DateTime.UtcNow, 15);

        Assert.Equal("Module", statistic.ModuleName);
        Assert.Equal("Page", statistic.PageName);
        Assert.Equal(15, statistic.SecondsDuration);
    }

    [Fact]
    public void Create_WithEmptyModuleName_Throws()
    {
        Assert.Throws<ArgumentException>(() => DomainStatistic.Create("", "Page", DateTime.UtcNow, 1));
    }

    [Fact]
    public void Create_WithNegativeDuration_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DomainStatistic.Create("Module", "Page", DateTime.UtcNow, -1));
    }

    [Fact]
    public void CreateWithDuration_ComputesElapsedSeconds()
    {
        DateTime started = new(2026, 6, 5, 10, 0, 0, DateTimeKind.Utc);
        DateTime ended = started.AddSeconds(42);

        var statistic = DomainStatistic.CreateWithDuration("Module", "Page", started, ended);

        Assert.Equal(42, statistic.SecondsDuration);
        Assert.Equal(ended, statistic.DateTime);
    }
}
