using Moq;
using PsychologyApp.Application.Reason;
using PsychologyApp.Presentation.Features.SearchPhysics;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class PhysicsSearchSessionTests
{
    [Fact]
    public void NewInstances_DoNotShareMutableSearchState()
    {
        Mock<IReasonSearchService> reasonSearch = new();
        PhysicsSearchCoordinator coordinator = new(reasonSearch.Object, TechniqueCatalogTestHelper.CreateGateway(), TechniqueCatalogTestHelper.CreateRecommendationService());
        PhysicsSearchSession first = new(coordinator);
        PhysicsSearchSession second = new(coordinator);

        Assert.NotSame(first, second);
        first.ResetSearchMatches();
        Assert.Equal(0, first.LoadedSearchCount);
        Assert.Equal(0, second.LoadedSearchCount);
    }

    [Fact]
    public void ResetSearchMatches_ClearsLoadedCount()
    {
        PhysicsSearchSession session = new(new PhysicsSearchCoordinator(new Mock<IReasonSearchService>().Object, TechniqueCatalogTestHelper.CreateGateway(), TechniqueCatalogTestHelper.CreateRecommendationService()));

        session.ResetSearchMatches();

        Assert.Equal(0, session.LoadedSearchCount);
    }
}
