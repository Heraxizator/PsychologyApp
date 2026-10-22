using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.Quot;
using PsychologyApp.Application.Technique;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Entities.Profile;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;
using PsychologyApp.Presentation.Shared.Services.Notifications;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Features.ManageProfile.Index;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Services.Clipboard;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed class UserViewModelTests
{
    public UserViewModelTests()
    {
        UserPreferences.UseInMemoryStorage();
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public void Constructor_BuildsFeaturedTechniquesFromOnboardingConcern()
    {
        Mock<IUserProgressService> progress = new();
        Mock<IQuotService> quotService = new();
        Mock<INavigationService> navigation = new();
        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState { OnboardingConcern = OnboardingConcernKeys.Body });

        UserViewModel viewModel = new(
            NullLogger<UserViewModel>.Instance,
            Options.Create(new AppSettings()),
            navigation.Object,
            new QuotesChangeNotifier(),
            new ProfileStatsLoader(progress.Object),
            new ProfileQuotesLoader(quotService.Object, new ProfileQuotesPresenter()),
            new ProfilePracticeHistoryLoader(progress.Object),
            new ProfileFeaturedTechniquesBuilder(
                preferences.Object,
                TechniqueCatalogTestHelper.CreateGateway(),
                TechniqueCatalogTestHelper.CreateRecommendationService()),
            TopViewModelTestHelpers.CreateQuoteCommandsFactory(quotService.Object),
            new ProfileScreenCoordinator(new UserProfileRefreshCoordinator()),
            TopViewModelTestHelpers.CreateLanguageReloader(quotService.Object),
            Mock.Of<IClinicalCareService>());

        Assert.Equal(4, viewModel.Techniques.Count);
        Assert.Equal("0", viewModel.TechniquesCompletedCount);
    }

    [Fact]
    public async Task RefreshAsync_SkipsQuotesWhenAlreadyLoaded()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.CountTechniqueCompletionsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0L);
        progress.Setup(p => p.CountTestResultsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0L);
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetLastTechniqueCompletionDateAsync(It.IsAny<CancellationToken>())).ReturnsAsync((DateTime?)null);
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        Mock<IQuotService> quotService = new();
        quotService.Setup(q => q.GetFavouritesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new QuotDTO { QuotId = 1, Text = "Quote", Title = "Author", IsFavourite = true }]);

        Mock<INavigationService> navigation = new();
        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState { OnboardingConcern = OnboardingConcernKeys.Body });

        ProfileQuotesLoader quotesLoader = new(quotService.Object, new ProfileQuotesPresenter());
        UserViewModel viewModel = new(
            NullLogger<UserViewModel>.Instance,
            Options.Create(new AppSettings()),
            navigation.Object,
            new QuotesChangeNotifier(),
            new ProfileStatsLoader(progress.Object),
            quotesLoader,
            new ProfilePracticeHistoryLoader(progress.Object),
            new ProfileFeaturedTechniquesBuilder(
                preferences.Object,
                TechniqueCatalogTestHelper.CreateGateway(),
                TechniqueCatalogTestHelper.CreateRecommendationService()),
            TopViewModelTestHelpers.CreateQuoteCommandsFactory(quotService.Object),
            new ProfileScreenCoordinator(new UserProfileRefreshCoordinator()),
            TopViewModelTestHelpers.CreateLanguageReloader(quotService.Object),
            Mock.Of<IClinicalCareService>());

        await viewModel.InitAsync();
        await Task.Delay(100);

        quotService.Verify(q => q.GetFavouritesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(quotesLoader.LoadedOnce);
        Assert.Single(viewModel.Quotes);

        await viewModel.RefreshAsync(forceQuotesReload: false);
        await Task.Delay(100);

        quotService.Verify(q => q.GetFavouritesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);

        await viewModel.RefreshAsync(forceQuotesReload: true);
        await Task.Delay(100);

        quotService.Verify(q => q.GetFavouritesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.AtLeast(2));
    }
}

public sealed class TechniquesViewModelTests
{
    public TechniquesViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    [Fact]
    public async Task TryOpenPendingTechniqueAsync_NavigatesWhenPendingExists()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetAtRiskStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetLastTechniqueCompletionDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateTime?)null);
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        progress.Setup(p => p.GetLastPracticeDatesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, DateTime>(StringComparer.Ordinal));
        progress.Setup(p => p.GetSessionDraftKeysAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal));
        progress.Setup(p => p.GetMostRecentTestResultAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestResultDTO?)null);

        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState { OnboardingConcern = OnboardingConcernKeys.Body });
        preferences.Setup(p => p.ConsumePendingTechnique()).Returns(TechniqueId.Spin);

        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.GoToTechniqueAsync(TechniqueId.Spin)).Returns(Task.CompletedTask);

        Mock<ITechniqueService> techniqueService = new();
        techniqueService.Setup(s => s.GetTechniquesPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        PracticeDashboardLoader dashboardLoader = CreateDashboardLoader(progress.Object, preferences.Object);
        TechniquesViewModel viewModel = CreateViewModel(
            techniqueService.Object,
            navigation.Object,
            dashboardLoader,
            progress.Object);

        await viewModel.TryOpenPendingTechniqueAsync();

        navigation.Verify(n => n.GoToTechniqueAsync(TechniqueId.Spin), Times.Once);
    }

    [Fact]
    public async Task RefreshOnAppear_DoesNotReloadCustomTechniquesPage()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);
        progress.Setup(p => p.GetAtRiskStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetLastTechniqueCompletionDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(DateTime.UtcNow);
        progress.Setup(p => p.GetMostRecentTestResultAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestResultDTO?)null);
        progress
            .Setup(p => p.GetLastPracticeDatesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, DateTime>(StringComparer.Ordinal));
        progress
            .Setup(p => p.GetSessionDraftKeysAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal));
        progress.Setup(p => p.GetSessionDraftAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState());

        Mock<INavigationService> navigation = new();
        Mock<ITechniqueService> techniqueService = new();
        techniqueService.Setup(s => s.GetTechniquesPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        PracticeDashboardLoader dashboardLoader = CreateDashboardLoader(progress.Object, preferences.Object);
        TechniquesViewModel viewModel = CreateViewModel(
            techniqueService.Object,
            navigation.Object,
            dashboardLoader,
            progress.Object);

        await viewModel.EnsureInitializedAsync();
        techniqueService.Invocations.Clear();

        await viewModel.RefreshOnAppearAsync();

        techniqueService.Verify(
            s => s.GetTechniquesPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
        Assert.Equal(2, viewModel.StreakDays);
    }

    [Fact]
    public async Task TechniqueMessage_AppliesSurgicalAdd()
    {
        Action<TechniqueMessage>? messageHandler = null;
        Mock<ITechniqueMessenger> messenger = new();
        messenger.Setup(m => m.Subscribe(It.IsAny<object>(), It.IsAny<Action<TechniqueMessage>>()))
            .Callback<object, Action<TechniqueMessage>>((_, handler) => messageHandler = handler);

        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetAtRiskStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetLastTechniqueCompletionDateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateTime?)null);
        progress.Setup(p => p.GetMostRecentTestResultAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestResultDTO?)null);
        progress
            .Setup(p => p.GetLastPracticeDatesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, DateTime>(StringComparer.Ordinal));
        progress
            .Setup(p => p.GetSessionDraftKeysAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal));
        progress.Setup(p => p.GetSessionDraftAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState());

        Mock<INavigationService> navigation = new();
        Mock<ITechniqueService> techniqueService = new();
        techniqueService.Setup(s => s.GetTechniquesPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        PracticeDashboardLoader dashboardLoader = CreateDashboardLoader(progress.Object, preferences.Object);
        TechniquesViewModel viewModel = CreateViewModel(
            techniqueService.Object,
            navigation.Object,
            dashboardLoader,
            progress.Object,
            messenger.Object);

        await viewModel.EnsureInitializedAsync();
        Assert.NotNull(messageHandler);

        techniqueService.Invocations.Clear();

        messageHandler!(new TechniqueMessage
        {
            MessageType = TechniqueMessageType.Add,
            Technique = new TechniqueDTO
            {
                TechniqueId = 42,
                Header = "Custom",
                Description = "Desc",
                Subject = "Theme",
                Author = "Author",
                Date = "Today"
            }
        });
        await Task.Delay(500);

        techniqueService.Verify(
            s => s.GetTechniquesPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
        Assert.True(viewModel.IsTechniquesGrouped);
        Assert.Contains(viewModel.TechniqueGroups.SelectMany(g => g), item => item.Id == 42);
    }

    private static PracticeDashboardLoader CreateDashboardLoader(
        IUserProgressService progress,
        IUserPreferencesStore preferences) =>
        new(
            progress,
            preferences,
            TechniqueCatalogTestHelper.CreateTodayRecommendationResolver(),
            Mock.Of<IClinicalCareService>());

    private static TechniquesViewModel CreateViewModel(
        ITechniqueService techniqueService,
        INavigationService navigationService,
        PracticeDashboardLoader dashboardLoader,
        IUserProgressService userProgressService,
        ITechniqueMessenger? techniqueMessenger = null)
    {
        Mock<IDatabaseReadySignal> databaseReady = new();
        databaseReady.Setup(d => d.WaitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        return new TechniquesViewModel(
            techniqueService,
            Mock.Of<IToastService>(),
            techniqueMessenger ?? Mock.Of<ITechniqueMessenger>(),
            navigationService,
            new TechniqueListBuilder(userProgressService, TechniqueCatalogTestHelper.CreateGateway()),
            databaseReady.Object,
            dashboardLoader,
            TechniqueCatalogTestHelper.CreateTodayRecommendationResolver(),
            new TechniquesListInitializer(),
            new PracticeClinicalDashboardEnricher(
                Mock.Of<IClinicalCareService>(),
                Mock.Of<Microsoft.Extensions.Logging.ILogger<PracticeClinicalDashboardEnricher>>()),
            Mock.Of<IClinicalCareService>(),
            Options.Create(new AppSettings()),
            Mock.Of<Microsoft.Extensions.Logging.ILogger<TechniquesViewModel>>());
    }
}

file static class TopViewModelTestHelpers
{
    public static QuoteItemCommandsFactory CreateQuoteCommandsFactory(IQuotService quotService) =>
        new(
            quotService,
            new QuotesChangeNotifier(),
            Mock.Of<IAppClipboardService>(),
            Mock.Of<IToastService>(),
            Options.Create(new AppSettings()),
            NullLogger<QuoteItemCommandsFactory>.Instance);

    public static LanguageContentReloader CreateLanguageReloader(IQuotService quotService) =>
        new(
            quotService,
            new CachedQuotContentProvider(Mock.Of<IQuotContentProvider>()),
            new CachedTestCatalogProvider(
                Mock.Of<ITestCatalogProvider>()),
            new CachedTechniqueCatalogProvider(Mock.Of<ITechniqueCatalogProvider>()),
            Mock.Of<ITechniqueCatalogService>());
}
