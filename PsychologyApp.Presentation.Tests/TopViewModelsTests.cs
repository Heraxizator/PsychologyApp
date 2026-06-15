using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.TechniqueService;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Core.Common;
using PsychologyApp.Presentation.Models.Profile;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Services.Preferences;
using PsychologyApp.Presentation.Services.Practice;
using PsychologyApp.Presentation.Services.Profile;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.ViewModels.Practice;
using PsychologyApp.Presentation.ViewModels.Profile;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class SettingsPreferencesPresenterTests
{
    [Fact]
    public void BuildState_PreservesOnboardingFlags()
    {
        SettingsPreferencesPresenter presenter = new();
        UserPreferencesState saved = new()
        {
            HasCompletedOnboarding = true,
            OnboardingConcern = OnboardingConcernKeys.Mood,
            Language = UserPreferences.DefaultLanguage
        };

        UserPreferencesState built = presenter.BuildState(
            UserPreferences.DefaultLanguage,
            UserPreferences.DefaultTheme,
            UserPreferences.DefaultColor,
            UserPreferences.DefaultForm,
            UserPreferences.DefaultSize,
            isBold: false,
            saved);

        Assert.True(built.HasCompletedOnboarding);
        Assert.Equal(OnboardingConcernKeys.Mood, built.OnboardingConcern);
    }
}

public sealed class SettingsViewModelTests
{
    [Fact]
    public async Task ReplayOnboardingCommand_ResetsOnboardingAndNavigates()
    {
        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.ShowOnboardingAsync()).Returns(Task.CompletedTask);
        Mock<IDialogService> dialog = new();
        InMemoryUserPreferencesStore store = new();
        store.CompleteOnboarding(OnboardingConcernKeys.Anxiety);

        SettingsViewModel viewModel = new(
            dialog.Object,
            navigation.Object,
            store,
            new SettingsPreferencesPresenter());

        viewModel.ReplayOnboardingCommand.Execute(null);
        await Task.Delay(200);

        Assert.False(store.Load().HasCompletedOnboarding);
        navigation.Verify(n => n.ShowOnboardingAsync(), Times.Once);
    }

    [Fact]
    public async Task ApplyCommand_SavesPreferencesAndShowsDialog()
    {
        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.GoBackAsync()).Returns(Task.CompletedTask);
        Mock<IDialogService> dialog = new();
        dialog.Setup(d => d.ShowAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(() => Task.CompletedTask);
        InMemoryUserPreferencesStore store = new();

        SettingsViewModel viewModel = new(
            dialog.Object,
            navigation.Object,
            store,
            new SettingsPreferencesPresenter());

        viewModel.ApplyCommand.Execute(null);
        await Task.Delay(200);

        dialog.Verify(d => d.ShowAsync(AppStrings.SettingsAppliedTitle, AppStrings.SettingsAppliedMessage), Times.Once);
        navigation.Verify(n => n.GoBackAsync(), Times.Once);
    }
}

public sealed class UserViewModelTests
{
    public UserViewModelTests()
    {
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
            new ProfileFeaturedTechniquesBuilder(preferences.Object),
            TopViewModelTestHelpers.CreateQuoteCommandsFactory(quotService.Object),
            new UserProfileRefreshCoordinator());

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
            new ProfileFeaturedTechniquesBuilder(preferences.Object),
            TopViewModelTestHelpers.CreateQuoteCommandsFactory(quotService.Object),
            new UserProfileRefreshCoordinator());

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
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState { OnboardingConcern = OnboardingConcernKeys.Body });
        preferences.Setup(p => p.ConsumePendingTechnique()).Returns(TechniqueId.Spin);

        Mock<INavigationService> navigation = new();
        navigation.Setup(n => n.GoToTechniqueAsync(TechniqueId.Spin)).Returns(Task.CompletedTask);

        Mock<ITechniqueService> techniqueService = new();
        techniqueService.Setup(s => s.GetTechniquesListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        PracticeDashboardLoader dashboardLoader = new(progress.Object, preferences.Object);
        TechniquesViewModel viewModel = CreateViewModel(
            techniqueService.Object,
            navigation.Object,
            dashboardLoader,
            preferences.Object,
            progress.Object);

        await viewModel.TryOpenPendingTechniqueAsync();

        navigation.Verify(n => n.GoToTechniqueAsync(TechniqueId.Spin), Times.Once);
    }

    [Fact]
    public async Task RecordMoodCommand_UpdatesSelectedLevel()
    {
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        progress.Setup(p => p.RecordMoodAsync(It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState());

        Mock<INavigationService> navigation = new();
        Mock<ITechniqueService> techniqueService = new();
        techniqueService.Setup(s => s.GetTechniquesListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        PracticeDashboardLoader dashboardLoader = new(progress.Object, preferences.Object);
        TechniquesViewModel viewModel = CreateViewModel(
            techniqueService.Object,
            navigation.Object,
            dashboardLoader,
            preferences.Object,
            progress.Object);

        viewModel.RecordMoodCommand.Execute(4);
        await Task.Delay(500);

        progress.Verify(p => p.RecordMoodAsync(4, It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TechniqueMessage_TriggersReload()
    {
        Action<TechniqueMessage>? messageHandler = null;
        Mock<ITechniqueMessenger> messenger = new();
        messenger.Setup(m => m.Subscribe(It.IsAny<object>(), It.IsAny<Action<TechniqueMessage>>()))
            .Callback<object, Action<TechniqueMessage>>((_, handler) => messageHandler = handler);

        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);
        progress.Setup(p => p.GetStreakDaysAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        progress.Setup(p => p.GetRecentTechniqueCompletionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        progress
            .Setup(p => p.GetLastPracticeDatesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, DateTime>(StringComparer.Ordinal));
        progress
            .Setup(p => p.GetSessionDraftKeysAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal));

        Mock<IUserPreferencesStore> preferences = new();
        preferences.Setup(p => p.Load()).Returns(new UserPreferencesState());

        Mock<INavigationService> navigation = new();
        Mock<ITechniqueService> techniqueService = new();
        techniqueService.Setup(s => s.GetTechniquesListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        PracticeDashboardLoader dashboardLoader = new(progress.Object, preferences.Object);
        TechniquesViewModel viewModel = CreateViewModel(
            techniqueService.Object,
            navigation.Object,
            dashboardLoader,
            preferences.Object,
            progress.Object,
            messenger.Object);

        await viewModel.EnsureInitializedAsync();
        Assert.NotNull(messageHandler);

        techniqueService.Invocations.Clear();

        messageHandler!(new TechniqueMessage { MessageType = TechniqueMessageType.Add });
        await Task.Delay(500);

        techniqueService.Verify(
            s => s.GetTechniquesListAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    private static TechniquesViewModel CreateViewModel(
        ITechniqueService techniqueService,
        INavigationService navigationService,
        PracticeDashboardLoader dashboardLoader,
        IUserPreferencesStore preferencesStore,
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
            userProgressService,
            new TechniqueListBuilder(userProgressService),
            databaseReady.Object,
            dashboardLoader,
            new TechniquesListInitializer(),
            Options.Create(new AppSettings()));
    }
}

file static class TopViewModelTestHelpers
{
    public static QuoteItemCommandsFactory CreateQuoteCommandsFactory(IQuotService quotService) =>
        new(
            quotService,
            new QuotesChangeNotifier(),
            Mock.Of<IToastService>(),
            Options.Create(new AppSettings()),
            NullLogger<QuoteItemCommandsFactory>.Instance);
}
