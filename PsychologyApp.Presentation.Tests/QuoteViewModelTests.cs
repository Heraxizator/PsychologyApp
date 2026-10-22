using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Shared.Abstractions;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Features.RunTests;
using PsychologyApp.Presentation.Shared.Services.Clipboard;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;
using PsychologyApp.Presentation.Features.ManageQuotes.Index;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed class QuoteViewModelTests
{
    [Fact]
    public async Task Reload_DoesNotCallLoadSingleAsync()
    {
        var quotService = CreateQuotServiceMock();
        QuoteViewModel viewModel = CreateViewModel(quotService.Object);
        await viewModel.EnsureInitializedAsync();
        await WaitForStateAsync(viewModel);

        quotService.Invocations.Clear();

        viewModel.Reload?.Execute(null);
        await WaitForStateAsync(viewModel);

        quotService.Verify(s => s.TryLoadSingleAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LikeCommand_NotifiesFavoritesChanged()
    {
        var quotService = CreateQuotServiceMock();
        var notifier = new QuotesChangeNotifier();
        int notifications = 0;
        notifier.FavoritesChanged += () => notifications++;

        QuoteViewModel viewModel = CreateViewModel(quotService.Object, notifier);
        await viewModel.EnsureInitializedAsync();
        await WaitForStateAsync(viewModel);

        Assert.NotEmpty(viewModel.DisplayItems);
        viewModel.DisplayItems[0].LikeCommand?.Execute(null);
        await Task.Delay(100);

        Assert.Equal(1, notifications);
        quotService.Verify(
            s => s.MarkAsFavouriteAsync(1, true, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_DoesNotLoadUntilEnsureInitialized()
    {
        Mock<IQuotService> quotService = CreateQuotServiceMock();
        QuoteViewModel viewModel = CreateViewModel(quotService.Object);

        Assert.False(viewModel.HasInitialized);
        Assert.False(viewModel.IsDone);
        quotService.Verify(
            s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void SearchQuery_WhenSet_EntersSearchMode()
    {
        QuoteViewModel viewModel = CreateViewModel(CreateQuotServiceMock().Object);

        Assert.False(viewModel.IsSearching);
        Assert.True(viewModel.IsFeedFiltersVisible);

        viewModel.SearchQuery = "wisdom";

        Assert.True(viewModel.IsSearching);
        Assert.False(viewModel.IsFeedFiltersVisible);
    }

    [Fact]
    public async Task SelectFeedCommand_Favorites_LoadsFavoritesFeed()
    {
        Mock<IQuotService> quotService = CreateQuotServiceMock();
        quotService
            .Setup(s => s.GetFavouritesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new QuotDTO
                {
                    QuotId = 99,
                    Text = "Favorite quote",
                    Title = "Author",
                    Theme = "wisdom",
                    IsFavourite = true,
                    IsReaded = false
                }
            ]);

        QuoteViewModel viewModel = CreateViewModel(quotService.Object);
        await viewModel.EnsureInitializedAsync();
        await WaitForStateAsync(viewModel);

        quotService.Invocations.Clear();
        viewModel.SelectFeedCommand.Execute("favorites");
        await WaitForStateAsync(viewModel);

        Assert.Equal(QuoteFeedMode.Favorites, viewModel.FeedMode);
        quotService.Verify(
            s => s.GetFavouritesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
        quotService.Verify(
            s => s.GetUnreadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SelectFeedCommand_ForYou_WhenThemedQuotesEmpty_ShowsForYouEmptyState()
    {
        Mock<IQuotService> quotService = CreateQuotServiceMock();
        quotService
            .Setup(s => s.GetUnreadByThemesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<QuotDTO>());

        QuoteViewModel viewModel = CreateViewModel(quotService.Object);
        await viewModel.EnsureInitializedAsync();
        await WaitForStateAsync(viewModel);

        quotService.Invocations.Clear();
        viewModel.SelectFeedCommand.Execute("for-you");
        // The first load already left the view model done, so wait for the switched feed itself.
        await WaitUntilAsync(() => viewModel.ShowForYouEmpty);

        Assert.Equal(QuoteFeedMode.ForYou, viewModel.FeedMode);
        Assert.Empty(viewModel.DisplayItems);
        quotService.Verify(
            s => s.GetUnreadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IQuotService> CreateQuotServiceMock()
    {
        var quotService = new Mock<IQuotService>();
        quotService
            .Setup(s => s.GetUnreadAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new QuotDTO
                {
                    QuotId = 1,
                    Text = "Test quote",
                    Title = "Author",
                    Theme = "wisdom",
                    IsFavourite = false,
                    IsReaded = false
                }
            ]);
        quotService
            .Setup(s => s.GetFavouritesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<QuotDTO>());
        quotService
            .Setup(s => s.GetUnreadByThemesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new QuotDTO
                {
                    QuotId = 2,
                    Text = "Themed quote",
                    Title = "Author",
                    Theme = "wisdom",
                    IsFavourite = false,
                    IsReaded = false
                }
            ]);
        quotService
            .Setup(s => s.EnsureThemedQuotesInFeedAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        quotService
            .Setup(s => s.TryLoadThemedSingleAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        quotService
            .Setup(s => s.TryLoadSingleAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        quotService
            .Setup(s => s.GetDailyQuoteAsync(It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((QuotDTO?)null);
        quotService
            .Setup(s => s.IsAllCaughtUpAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        quotService
            .Setup(s => s.MarkAsReadedAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        quotService
            .Setup(s => s.MarkAsFavouriteAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return quotService;
    }

    [Fact]
    public async Task A_language_only_previewed_in_settings_does_not_reseed_the_feed()
    {
        var quotService = CreateQuotServiceMock();
        quotService
            .Setup(s => s.ReseedFeedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        InMemoryUserPreferencesStore preferences = new();
        LanguageContentReloader languageReloader = CreateLanguageReloader(quotService.Object, preferences);

        QuoteViewModel viewModel = CreateViewModel(quotService.Object, languageReloader: languageReloader, preferences: preferences);
        await viewModel.EnsureInitializedAsync();
        await WaitForStateAsync(viewModel);
        quotService.Invocations.Clear();

        preferences.ApplyPreview(preferences.Load() with { Language = "en" });
        await languageReloader.EnsureReloadedAsync();

        quotService.Verify(
            s => s.ReseedFeedAsync(LanguageContentReloader.DefaultQuoteFeedCount, It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task A_saved_language_change_reseeds_the_feed_in_the_new_language()
    {
        var quotService = CreateQuotServiceMock();
        quotService
            .Setup(s => s.ReseedFeedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        InMemoryUserPreferencesStore preferences = new();
        LanguageContentReloader languageReloader = CreateLanguageReloader(quotService.Object, preferences);

        QuoteViewModel viewModel = CreateViewModel(quotService.Object, languageReloader: languageReloader, preferences: preferences);
        await viewModel.EnsureInitializedAsync();
        await WaitForStateAsync(viewModel);
        quotService.Invocations.Clear();

        preferences.Save(preferences.Load() with { Language = "en" });
        await languageReloader.EnsureReloadedAsync();

        quotService.Verify(
            s => s.ReseedFeedAsync(LanguageContentReloader.DefaultQuoteFeedCount, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static QuoteViewModel CreateViewModel(
        IQuotService quotService,
        IQuotesChangeNotifier? notifier = null,
        LanguageContentReloader? languageReloader = null,
        InMemoryUserPreferencesStore? preferences = null)
    {
        var navigation = new Mock<INavigation>();
        var toast = new Mock<IToastService>();
        IOptions<AppSettings> settings = Options.Create(new AppSettings
        {
            SmallTimeoutMs = 5000,
            MiddleTimeoutMs = 10000
        });
        preferences ??= new InMemoryUserPreferencesStore();
        Mock<IUserProgressService> progress = new();
        progress.Setup(p => p.GetRecentMoodsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        return new QuoteViewModel(
            new TestNavigationService(navigation.Object),
            quotService,
            Mock.Of<IQuoteSearchService>(),
            NullLogger<QuoteViewModel>.Instance,
            toast.Object,
            settings,
            new QuoteFeedCoordinator(progress.Object, preferences),
            new QuoteItemCommandsFactory(
                quotService,
                notifier ?? new QuotesChangeNotifier(),
                Mock.Of<IAppClipboardService>(),
                toast.Object,
                settings,
                NullLogger<QuoteItemCommandsFactory>.Instance),
            TestDatabaseReady.CreateSignaled(),
            languageReloader ?? CreateLanguageReloader(quotService, preferences),
            preferences);
    }

    private static LanguageContentReloader CreateLanguageReloader(IQuotService quotService, IUserPreferencesStore preferences) =>
        new(
            quotService,
            new CachedQuotContentProvider(Mock.Of<IQuotContentProvider>()),
            new CachedTestCatalogProvider(Mock.Of<ITestCatalogProvider>()),
            new CachedTechniqueCatalogProvider(Mock.Of<ITechniqueCatalogProvider>()),
            Mock.Of<ITechniqueCatalogService>(),
            preferences);

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        for (int i = 0; i < 100 && !condition(); i++)
        {
            await Task.Delay(50);
        }

        Assert.True(condition());
    }

    private static async Task WaitForStateAsync(QuoteViewModel viewModel)
    {
        for (int i = 0; i < 100; i++)
        {
            if (viewModel.IsDone || viewModel.IsFail)
            {
                return;
            }

            await Task.Delay(50);
        }

        throw new TimeoutException("QuoteViewModel did not reach a terminal state.");
    }
}
