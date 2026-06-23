using Moq;
using PsychologyApp.Application.Analytics;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Statistic;
using Xunit;

namespace PsychologyApp.Application.Tests.Analytics;

public class PageAnalyticsServiceTests
{
    [Fact]
    public async Task TrackPageVisitAsync_PersistsStatistic()
    {
        var statisticService = new Mock<IStatisticService>();
        var service = new PageAnalyticsService(statisticService.Object, TimeProvider.System);
        var startedAt = DateTime.Now.AddSeconds(-30);

        await service.TrackPageVisitAsync("Практик", "Тест", startedAt);

        statisticService.Verify(
            s => s.AddSingleAsync(It.Is<StatisticDTO>(dto =>
                dto.ModuleName == "Практик" &&
                dto.PageName == "Тест" &&
                dto.SecondsDuration >= 0),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
