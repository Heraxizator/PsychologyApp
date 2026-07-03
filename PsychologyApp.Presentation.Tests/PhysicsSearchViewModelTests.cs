using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Reason;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.SearchPhysics;
using PsychologyApp.Presentation.Pages.SearchPhysics.PhysicsSearch;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed class PhysicsSearchViewModelTests
{
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

        PhysicsSearchCoordinator coordinator = new(reasonSearch.Object, TechniqueCatalogTestHelper.CreateGateway(), TechniqueCatalogTestHelper.CreateRecommendationService());
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

        PhysicsSearchCoordinator coordinator = new(reasonSearch.Object, TechniqueCatalogTestHelper.CreateGateway(), TechniqueCatalogTestHelper.CreateRecommendationService());
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
        PhysicsSearchCoordinator coordinator = new(reasonSearch.Object, TechniqueCatalogTestHelper.CreateGateway(), TechniqueCatalogTestHelper.CreateRecommendationService());
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

    [Fact]
    public async Task RefreshLocalizedProperties_DoesNotReloadReasonsWhenOnlyThemeChanges()
    {
        Mock<IReasonSearchService> reasonSearch = new();
        reasonSearch
            .Setup(r => r.LoadReasonsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        PhysicsSearchViewModel viewModel = CreateViewModel(reasonSearch.Object);
        await viewModel.EnsureInitializedAsync();
        await WaitUntilDoneAsync(viewModel);
        reasonSearch.Invocations.Clear();

        UserPreferences.ApplyPreview(new UserPreferencesState
        {
            Language = UserPreferences.DefaultLanguage,
            Theme = "dark",
            Color = UserPreferences.DefaultColor,
            Form = UserPreferences.DefaultForm,
            Size = UserPreferences.DefaultSize,
            IsBold = false,
            HasCompletedOnboarding = true,
            OnboardingConcern = "explore"
        });

        await Task.Delay(200);
        reasonSearch.Verify(r => r.LoadReasonsAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RefreshLocalizedProperties_WhenPreviewLanguageChanges_DoesNotReloadReasons()
    {
        Mock<IReasonSearchService> reasonSearch = new();
        reasonSearch
            .Setup(r => r.LoadReasonsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        PhysicsSearchViewModel viewModel = CreateViewModel(reasonSearch.Object);
        await viewModel.EnsureInitializedAsync();
        await WaitUntilDoneAsync(viewModel);
        reasonSearch.Invocations.Clear();

        try
        {
            UserPreferences.ApplyPreview(new UserPreferencesState
            {
                Language = "en",
                Theme = UserPreferences.DefaultTheme,
                Color = UserPreferences.DefaultColor,
                Form = UserPreferences.DefaultForm,
                Size = UserPreferences.DefaultSize,
                IsBold = false,
                HasCompletedOnboarding = true,
                OnboardingConcern = "explore"
            });
            await Task.Delay(200);

            reasonSearch.Verify(r => r.LoadReasonsAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        finally
        {
            AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
        }
    }

    [Fact]
    public async Task RefreshLocalizedProperties_ReloadsReasonsWhenPersistedLanguageChanges()
    {
        Mock<IReasonSearchService> reasonSearch = new();
        reasonSearch
            .Setup(r => r.LoadReasonsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        PhysicsSearchViewModel viewModel = CreateViewModel(reasonSearch.Object);

        try
        {
            await viewModel.EnsureInitializedAsync();
            await WaitUntilDoneAsync(viewModel);
            reasonSearch.Invocations.Clear();

            UserPreferences.Save(new UserPreferencesState
            {
                Language = "en",
                Theme = UserPreferences.DefaultTheme,
                Color = UserPreferences.DefaultColor,
                Form = UserPreferences.DefaultForm,
                Size = UserPreferences.DefaultSize,
                IsBold = false,
                HasCompletedOnboarding = true,
                OnboardingConcern = "explore"
            });
            UserPreferences.ApplyAll();
            await WaitUntilDoneAsync(viewModel);

            reasonSearch.Verify(r => r.LoadReasonsAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
        finally
        {
            UserPreferences.Save(new UserPreferencesState
            {
                Language = UserPreferences.DefaultLanguage,
                Theme = UserPreferences.DefaultTheme,
                Color = UserPreferences.DefaultColor,
                Form = UserPreferences.DefaultForm,
                Size = UserPreferences.DefaultSize,
                IsBold = false,
                HasCompletedOnboarding = true,
                OnboardingConcern = "explore"
            });
            UserPreferences.ApplyAll();
            AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
        }
    }

    private static PhysicsSearchViewModel CreateViewModel(IReasonSearchService reasonSearch)
    {
        PhysicsSearchCoordinator coordinator = new(reasonSearch, TechniqueCatalogTestHelper.CreateGateway(), TechniqueCatalogTestHelper.CreateRecommendationService());
        PhysicsSearchSession session = new(coordinator);
        Mock<INavigation> navigation = new();

        return new PhysicsSearchViewModel(
            reasonSearch,
            coordinator,
            session,
            NullLogger<PhysicsSearchViewModel>.Instance,
            Options.Create(new AppSettings()),
            new TestNavigationService(navigation.Object),
            TestDatabaseReady.CreateSignaled());
    }

    private static async Task WaitUntilDoneAsync(PhysicsSearchViewModel viewModel)
    {
        for (int attempt = 0; attempt < 50 && !viewModel.IsDone && !viewModel.IsFail; attempt++)
        {
            await Task.Delay(50);
        }
    }
}
