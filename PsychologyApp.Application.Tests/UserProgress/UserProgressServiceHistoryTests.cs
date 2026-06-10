using Moq;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using Xunit;

namespace PsychologyApp.Application.Tests.UserProgress;

public sealed class UserProgressServiceHistoryTests
{
    [Fact]
    public async Task GetTestResultHistoryAsync_DelegatesToRepository()
    {
        var repository = new Mock<IUserProgressRepository>();
        var expected = new List<TestResultDTO>
        {
            new() { TestId = "test_a", Summary = "First" },
            new() { TestId = "test_a", Summary = "Second" }
        };

        repository
            .Setup(r => r.GetTestResultHistoryAsync("test_a", 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var service = new UserProgressService(repository.Object);
        IReadOnlyList<TestResultDTO> actual = await service.GetTestResultHistoryAsync("test_a", 5);

        Assert.Equal(2, actual.Count);
        repository.Verify(r => r.GetTestResultHistoryAsync("test_a", 5, It.IsAny<CancellationToken>()), Times.Once);
    }
}
