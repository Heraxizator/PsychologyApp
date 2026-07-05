using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Features.ManageQuotes.Index;
using PsychologyApp.Presentation.Shared.Services.Clipboard;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class QuoteSearchControllerTests
{
    [Fact]
    public async Task SearchAsync_ReplacesDisplayItemsWithResults()
    {
        QuoteFeedState feedState = new();
        feedState.SetFeedItems([CreateFeedItem("Feed quote")]);
        Mock<IQuoteSearchService> searchService = new();
        searchService
            .Setup(s => s.SearchCatalogAsync("calm", It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new QuotSeed("Seneca", "Calm quote", "calm"),
                new QuotSeed("Marcus", "Peace quote", "calm")
            ]);

        QuoteSearchController controller = CreateController(feedState, searchService.Object);
        controller.Query = "calm";
        await Task.Delay(350);

        Assert.Equal(2, feedState.DisplayItems.Count);
        Assert.Equal("Calm quote", feedState.DisplayItems[0].Text);
        Assert.Equal("Peace quote", feedState.DisplayItems[1].Text);
    }

    [Fact]
    public async Task ClearQuery_RestoresFeedDisplay()
    {
        QuoteFeedState feedState = new();
        feedState.SetFeedItems([CreateFeedItem("Feed quote")]);
        bool searchCleared = false;
        Mock<IQuoteSearchService> searchService = new();
        searchService
            .Setup(s => s.SearchCatalogAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new QuotSeed("A", "Result", "wisdom")]);

        QuoteSearchController controller = CreateController(
            feedState,
            searchService.Object,
            onSearchCleared: () => searchCleared = true);
        controller.Query = "test";
        await Task.Delay(350);
        controller.Query = string.Empty;
        await Task.Delay(50);

        Assert.True(searchCleared);
        Assert.Single(feedState.DisplayItems);
        Assert.Equal("Feed quote", feedState.DisplayItems[0].Text);
    }

    [Fact]
    public async Task RapidQueryChanges_OnlyLatestSearchIsApplied()
    {
        QuoteFeedState feedState = new();
        Mock<IQuoteSearchService> searchService = new();
        searchService
            .Setup(s => s.SearchCatalogAsync("first", It.IsAny<CancellationToken>()))
            .ReturnsAsync([new QuotSeed("A", "First", "wisdom")]);
        searchService
            .Setup(s => s.SearchCatalogAsync("second", It.IsAny<CancellationToken>()))
            .ReturnsAsync([new QuotSeed("B", "Second", "calm")]);

        QuoteSearchController controller = CreateController(feedState, searchService.Object);
        controller.Query = "first";
        controller.Query = "second";
        await Task.Delay(350);

        Assert.Single(feedState.DisplayItems);
        Assert.Equal("Second", feedState.DisplayItems[0].Text);
        searchService.Verify(
            s => s.SearchCatalogAsync("first", It.IsAny<CancellationToken>()),
            Times.Never);
        searchService.Verify(
            s => s.SearchCatalogAsync("second", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static QuoteSearchController CreateController(
        QuoteFeedState feedState,
        IQuoteSearchService searchService,
        Action? onSearchCleared = null)
    {
        QuoteItemCommandsFactory commandsFactory = new(
            Mock.Of<IQuotService>(),
            new QuotesChangeNotifier(),
            Mock.Of<IAppClipboardService>(),
            Mock.Of<IToastService>(),
            Microsoft.Extensions.Options.Options.Create(new PsychologyApp.Application.Configuration.AppSettings()),
            NullLogger<QuoteItemCommandsFactory>.Instance);

        return new QuoteSearchController(
            searchService,
            commandsFactory,
            feedState,
            NullLogger.Instance,
            onStateChanged: () => { },
            onSearchCleared: onSearchCleared ?? (() => { }),
            onFail: () => { });
    }

    private static QuoteItem CreateFeedItem(string text) =>
        new()
        {
            Id = 1,
            Text = text,
            Author = "Author",
            Theme = "wisdom"
        };
}
