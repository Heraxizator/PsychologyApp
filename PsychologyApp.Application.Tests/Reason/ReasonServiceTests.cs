using Moq;
using PsychologyApp.Application.Abstractions.Integration;
using AppReasonService = PsychologyApp.Application.Services.ReasonService.ReasonService;
using DomainReason = PsychologyApp.Domain.Entities.Reason;
using Xunit;

namespace PsychologyApp.Application.Tests.Reasons;

public class ReasonServiceTests
{
    [Fact]
    public async Task GetReasonsAsync_UsesPageAndPageSize()
    {
        var reasons = Enumerable.Range(1, 10)
            .Select(i => DomainReason.Create($"Title{i}", $"Sub{i}", $"Sol{i}"))
            .ToList();

        var provider = new Mock<IReasonContentProvider>();
        provider.Setup(p => p.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int page, int pageSize, CancellationToken _) =>
                reasons.Skip(page * pageSize).Take(pageSize).ToList());

        var service = new AppReasonService(provider.Object);

        var page0 = (await service.GetReasonsAsync(0, 3)).ToList();
        var page1 = (await service.GetReasonsAsync(1, 3)).ToList();

        Assert.Equal(3, page0.Count);
        Assert.Equal("Title1", page0[0].Title);
        Assert.Equal(3, page1.Count);
        Assert.Equal("Title4", page1[0].Title);
    }
}
