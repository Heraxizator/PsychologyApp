using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.ReasonSearch;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Physics;
using PsychologyApp.Presentation.ViewModels.Physics;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class PhysicsSearchViewModelTests
{
    public PhysicsSearchViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task ExecuteSearch_WhenSearchFails_ResetsIsSearching()
    {
        Mock<IReasonSearchService> reasonSearch = new();
        reasonSearch
            .Setup(r => r.LoadReasonsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        reasonSearch
            .Setup(r => r.Search(It.IsAny<IReadOnlyList<ReasonDTO>>(), It.IsAny<string>()))
            .Throws(new InvalidOperationException("Search failed"));

        PhysicsSearchCoordinator coordinator = new(reasonSearch.Object);
        PhysicsSearchSession session = new(coordinator);
        Mock<INavigation> navigation = new();
        TestNavigationService navigationService = new(navigation.Object);

        PhysicsSearchViewModel viewModel = new(
            reasonSearch.Object,
            coordinator,
            session,
            NullLogger<PhysicsSearchViewModel>.Instance,
            Options.Create(new AppSettings()),
            navigationService,
            TestDatabaseReady.CreateSignaled());

        await viewModel.EnsureInitializedAsync();
        await WaitUntilDoneAsync(viewModel);

        viewModel.SearchText = "stress";
        viewModel.SearchCommand.Execute(null);
        await Task.Delay(500);

        Assert.False(viewModel.IsSearching);
    }

    [Fact]
    public async Task ReloadAsync_UsesFreshSessionWithoutPreviousMatches()
    {
        Mock<IReasonSearchService> reasonSearch = new();
        reasonSearch
            .Setup(r => r.LoadReasonsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new ReasonDTO { Title = "Stress relief", Subtitle = "Sub", Solution = "Solution" }
            ]);
        reasonSearch
            .Setup(r => r.Search(It.IsAny<IReadOnlyList<ReasonDTO>>(), "stress"))
            .Returns([new RankedReason(new ReasonDTO { Title = "Stress relief" }, 3)]);

        PhysicsSearchCoordinator coordinator = new(reasonSearch.Object);
        PhysicsSearchSession session = new(coordinator);
        Mock<INavigation> navigation = new();
        TestNavigationService navigationService = new(navigation.Object);

        PhysicsSearchViewModel viewModel = new(
            reasonSearch.Object,
            coordinator,
            session,
            NullLogger<PhysicsSearchViewModel>.Instance,
            Options.Create(new AppSettings()),
            navigationService,
            TestDatabaseReady.CreateSignaled());

        await viewModel.EnsureInitializedAsync();
        await WaitUntilDoneAsync(viewModel);
        viewModel.SearchText = "stress";
        await Task.Delay(400);
        Assert.True(viewModel.ResultsObservableCollection.Count > 0);

        session.ResetSearchMatches();
        Assert.NotNull(viewModel.Reload);
        viewModel.Reload.Execute(null);
        await WaitUntilDoneAsync(viewModel);
        viewModel.SearchText = "stress";
        await Task.Delay(400);

        Assert.True(viewModel.IsDone);
    }

    [Fact]
    public void Constructor_DoesNotLoadUntilEnsureInitialized()
    {
        Mock<IReasonSearchService> reasonSearch = new();
        PhysicsSearchCoordinator coordinator = new(reasonSearch.Object);
        PhysicsSearchSession session = new(coordinator);
        Mock<INavigation> navigation = new();

        PhysicsSearchViewModel viewModel = new(
            reasonSearch.Object,
            coordinator,
            session,
            NullLogger<PhysicsSearchViewModel>.Instance,
            Options.Create(new AppSettings()),
            new TestNavigationService(navigation.Object),
            TestDatabaseReady.CreateSignaled());

        Assert.False(viewModel.HasInitialized);
        reasonSearch.Verify(r => r.LoadReasonsAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static async Task WaitUntilDoneAsync(PhysicsSearchViewModel viewModel)
    {
        for (int attempt = 0; attempt < 50 && !viewModel.IsDone && !viewModel.IsFail; attempt++)
        {
            await Task.Delay(50);
        }
    }
}
