using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Application.Services.ReasonService;
using PsychologyApp.Presentation.Abstractions;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.Services.Tests;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.ViewModels.Motivator;
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

        quotService.Verify(s => s.LoadSingleAsync(It.IsAny<CancellationToken>()), Times.Never);
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

        Assert.NotEmpty(viewModel.QuotesObservableCollection);
        viewModel.QuotesObservableCollection[0].LikeCommand?.Execute(null);
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

    private static Mock<IQuotService> CreateQuotServiceMock()
    {
        var quotService = new Mock<IQuotService>();
        quotService
            .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new QuotDTO
                {
                    QuotId = 1,
                    Text = "Test quote",
                    Title = "Author",
                    IsFavourite = false,
                    IsReaded = false
                }
            ]);
        quotService
            .Setup(s => s.GetFavouritesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<QuotDTO>());
        quotService
            .Setup(s => s.LoadSingleAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        quotService
            .Setup(s => s.MarkAsReadedAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        quotService
            .Setup(s => s.MarkAsFavouriteAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return quotService;
    }

    [Fact]
    public async Task RefreshLocalizedProperties_WhenPreviewLanguageChanges_DoesNotReseedFeed()
    {
        var quotService = CreateQuotServiceMock();
        quotService
            .Setup(s => s.ReseedFeedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        LanguageContentReloader languageReloader = CreateLanguageReloader(quotService.Object);

        QuoteViewModel viewModel = CreateViewModel(quotService.Object, languageReloader: languageReloader);
        await viewModel.EnsureInitializedAsync();
        await WaitForStateAsync(viewModel);

        quotService.Invocations.Clear();

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
            await WaitForStateAsync(viewModel);

            quotService.Verify(
                s => s.ReseedFeedAsync(LanguageContentReloader.DefaultQuoteFeedCount, It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
        }
    }

    [Fact]
    public async Task RefreshLocalizedProperties_WhenPersistedLanguageChanges_ReloadsFeed()
    {
        var quotService = CreateQuotServiceMock();
        quotService
            .Setup(s => s.ReseedFeedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        LanguageContentReloader languageReloader = CreateLanguageReloader(quotService.Object);

        QuoteViewModel viewModel = CreateViewModel(quotService.Object, languageReloader: languageReloader);
        await viewModel.EnsureInitializedAsync();
        await WaitForStateAsync(viewModel);

        quotService.Invocations.Clear();

        try
        {
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
            await languageReloader.EnsureReloadedAsync();
            await WaitForStateAsync(viewModel);

            quotService.Verify(
                s => s.ReseedFeedAsync(LanguageContentReloader.DefaultQuoteFeedCount, It.IsAny<CancellationToken>()),
                Times.Once);
            quotService.Verify(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
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

    private static QuoteViewModel CreateViewModel(
        IQuotService quotService,
        IQuotesChangeNotifier? notifier = null,
        LanguageContentReloader? languageReloader = null)
    {
        var navigation = new Mock<INavigation>();
        var toast = new Mock<IToastService>();
        IOptions<AppSettings> settings = Options.Create(new AppSettings
        {
            SmallTimeoutMs = 5000,
            MiddleTimeoutMs = 10000
        });

        return new QuoteViewModel(
            new TestNavigationService(navigation.Object),
            quotService,
            NullLogger<QuoteViewModel>.Instance,
            settings,
            new QuoteFeedCoordinator(),
            new QuoteItemCommandsFactory(
                quotService,
                notifier ?? new QuotesChangeNotifier(),
                toast.Object,
                settings,
                NullLogger<QuoteItemCommandsFactory>.Instance),
            new QuoteFeedLoader(),
            TestDatabaseReady.CreateSignaled(),
            languageReloader ?? CreateLanguageReloader(quotService));
    }

    private static LanguageContentReloader CreateLanguageReloader(IQuotService quotService) =>
        new(
            quotService,
            new CachedReasonContentProvider(Mock.Of<IReasonContentProvider>()),
            new CachedQuotContentProvider(Mock.Of<IQuotContentProvider>()),
            new CachedTestCatalogService(
                new TestCatalogService(Mock.Of<ITestAssetReader>(), NullLogger<TestCatalogService>.Instance),
                NullLogger<CachedTestCatalogService>.Instance));

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
