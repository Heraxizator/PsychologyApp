using Moq;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Reason;
using PsychologyApp.Presentation.Entities.Physics;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.SearchPhysics;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class PhysicsSearchCoordinatorTests
{
    [Fact]
    public async Task SearchAsync_ReturnsMappedResultsWithoutThreadPool()
    {
        Mock<IReasonSearchService> reasonSearch = new();
        reasonSearch
            .Setup(r => r.Search(It.IsAny<IReadOnlyList<ReasonDTO>>(), "stress"))
            .Returns([new RankedReason(new ReasonDTO { Title = "Stress relief" }, 3)]);

        PhysicsSearchCoordinator coordinator = new(reasonSearch.Object, TechniqueCatalogTestHelper.CreateGateway(), TechniqueCatalogTestHelper.CreateRecommendationService());
        Mock<INavigation> navigation = new();
        TestNavigationService navigationService = new(navigation.Object);
        List<ReasonDTO> reasons = [new ReasonDTO { Title = "Stress relief" }];

        IReadOnlyList<PhysicsReasonItem> results = await coordinator.SearchAsync(
            reasons,
            "stress",
            navigationService,
            (reason, _, _) => new PhysicsReasonItem { Title = reason.Title ?? string.Empty },
            CancellationToken.None);

        Assert.Single(results);
        Assert.Equal("Stress relief", results[0].Title);
    }

    [Fact]
    public async Task SearchAsync_HonorsCancellation()
    {
        Mock<IReasonSearchService> reasonSearch = new();
        reasonSearch
            .Setup(r => r.Search(It.IsAny<IReadOnlyList<ReasonDTO>>(), It.IsAny<string>()))
            .Returns([]);

        PhysicsSearchCoordinator coordinator = new(reasonSearch.Object, TechniqueCatalogTestHelper.CreateGateway(), TechniqueCatalogTestHelper.CreateRecommendationService());
        using CancellationTokenSource cts = new();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            coordinator.SearchAsync(
                [],
                "stress",
                new TestNavigationService(new Mock<INavigation>().Object),
                (reason, _, _) => new PhysicsReasonItem { Title = reason.Title ?? string.Empty },
                cts.Token));
    }
}
