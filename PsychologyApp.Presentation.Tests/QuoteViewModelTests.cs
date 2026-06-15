using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.ViewModels.Motivator;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class QuoteViewModelTests
{
    public QuoteViewModelTests()
    {
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

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

    private static QuoteViewModel CreateViewModel(
        IQuotService quotService,
        IQuotesChangeNotifier? notifier = null)
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
            TestDatabaseReady.CreateSignaled());
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
