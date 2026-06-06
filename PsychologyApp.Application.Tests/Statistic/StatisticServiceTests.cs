using Moq;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.Statistic;
using DomainStatistic = PsychologyApp.Domain.Entities.Statistic;
using Xunit;

namespace PsychologyApp.Application.Tests.Statistic;

public class StatisticServiceTests
{
    [Fact]
    public async Task AddSingleAsync_PersistsStatistic()
    {
        var repository = new Mock<IStatisticRepository>();
        var service = new StatisticService(repository.Object);
        var dto = new StatisticDTO
        {
            ModuleName = "Module",
            PageName = "Page",
            DateTime = DateTime.UtcNow,
            SecondsDuration = 10
        };

        await service.AddSingleAsync(dto);

        repository.Verify(
            r => r.AddAsync(It.Is<DomainStatistic>(s => s.ModuleName == "Module" && s.PageName == "Page"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
