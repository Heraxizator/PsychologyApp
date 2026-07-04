using Moq;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Quot;
using DomainQuot = PsychologyApp.Domain.Entities.Quot;
using Xunit;

namespace PsychologyApp.Application.Tests.Quot;

public class QuotServiceTests
{
    [Fact]
    public async Task LoadSingleAsync_LoadsFromProviderAndPersistsQuot()
    {
        IReadOnlyList<QuotSeed> seeds =
        [
            new QuotSeed("Seneca", "Luck is what happens when preparation meets opportunity.", "wisdom")
        ];

        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetExistingTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(seeds);

        var service = QuotServiceTestFactory.Create(repository, provider);

        await service.LoadSingleAsync();

        provider.Verify(p => p.LoadAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(
            r => r.AddManyAsync(
                It.Is<IReadOnlyList<DomainQuot>>(items =>
                    items.Count == 1 &&
                    items[0].Text == seeds[0].Text &&
                    items[0].Title == seeds[0].Author),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LoadSingleAsync_WhenCatalogEmpty_Throws()
    {
        var repository = new Mock<IQuotRepository>();
        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<QuotSeed>());

        var service = QuotServiceTestFactory.Create(repository, provider);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.LoadSingleAsync());
    }

    [Fact]
    public async Task LoadSingleAsync_SkipsTextsAlreadyInDatabase()
    {
        IReadOnlyList<QuotSeed> seeds =
        [
            new QuotSeed("A", "First quote", "wisdom"),
            new QuotSeed("B", "Second quote", "calm"),
        ];

        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetExistingTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(["First quote"]);
        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(seeds);

        var service = QuotServiceTestFactory.Create(repository, provider);

        await service.LoadSingleAsync();

        repository.Verify(
            r => r.AddManyAsync(
                It.Is<IReadOnlyList<DomainQuot>>(items =>
                    items.Count == 1 &&
                    items[0].Text == "Second quote"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LoadSingleAsync_WhenAllSeedsUsed_ClearsDatabaseAndReseeds()
    {
        IReadOnlyList<QuotSeed> seeds =
        [
            new QuotSeed("A", "Only quote", "wisdom"),
        ];

        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetExistingTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(["Only quote"]);
        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(seeds);

        var service = QuotServiceTestFactory.Create(repository, provider);

        await service.LoadSingleAsync();

        repository.Verify(r => r.DeleteAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(
            r => r.AddManyAsync(
                It.Is<IReadOnlyList<DomainQuot>>(items =>
                    items.Count == 1 &&
                    items[0].Text == "Only quote"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MarkAsFavouriteAsync_UpdatesQuotAndStoresQuoteText()
    {
        var quot = DomainQuot.Create("Author", "Text", "Theme", false, false);
        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quot);
        repository.Setup(r => r.EditAsync(quot, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var favoriteStore = new Mock<IFavoriteQuoteTextStore>();
        favoriteStore.Setup(s => s.GetTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal));

        var service = QuotServiceTestFactory.Create(repository, favoriteTextStore: favoriteStore);

        await service.MarkAsFavouriteAsync(1, true);

        repository.Verify(r => r.EditAsync(It.Is<DomainQuot>(q => q.IsFavourite), It.IsAny<CancellationToken>()), Times.Once);
        favoriteStore.Verify(s => s.AddTextAsync("Text", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsFavouriteAsync_WhenEditFails_ThrowsNotFound()
    {
        var quot = DomainQuot.Create("Author", "Text", "Theme", false, false);
        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quot);
        repository.Setup(r => r.EditAsync(quot, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PsychologyApp.Application.Exceptions.PersistenceException("not updated"));

        var service = QuotServiceTestFactory.Create(repository);

        await Assert.ThrowsAsync<PsychologyApp.Application.Exceptions.PersistenceException>(
            () => service.MarkAsFavouriteAsync(1, true));
    }

    [Fact]
    public async Task ReseedFeedAsync_RestoresFavoriteQuotesByText()
    {
        IReadOnlyList<QuotSeed> seeds =
        [
            new QuotSeed("Seneca", "Quote A", "wisdom"),
            new QuotSeed("Marcus Aurelius", "Quote B", "resilience")
        ];

        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetExistingTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        repository.Setup(r => r.GetFavoriteTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        repository.Setup(r => r.GetByTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainQuot?)null);
        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(seeds);
        var catalogLookup = new Mock<IQuoteCatalogLookup>();
        catalogLookup.Setup(l => l.TryGetIndexByTextAsync("Quote B", It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        catalogLookup.Setup(l => l.GetSeedAtAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(seeds[1]);
        var favoriteStore = new Mock<IFavoriteQuoteTextStore>();
        favoriteStore.Setup(s => s.GetTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal) { "Quote B" });
        favoriteStore.Setup(s => s.GetLegacyIndicesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<int>());
        favoriteStore.Setup(s => s.SaveTextsAsync(It.IsAny<IReadOnlySet<string>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = QuotServiceTestFactory.Create(repository, provider, catalogLookup, favoriteStore);

        await service.ReseedFeedAsync(1);

        repository.Verify(r => r.DeleteAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(
            r => r.AddAsync(
                It.Is<DomainQuot>(q => q.Text == "Quote B" && q.IsFavourite),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ReseedFeedAsync_RestoresOnlyStoredFavoriteTexts()
    {
        IReadOnlyList<QuotSeed> seeds =
        [
            new QuotSeed("Seneca", "Quote A", "wisdom"),
            new QuotSeed("Marcus Aurelius", "Quote B", "resilience")
        ];

        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetExistingTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        repository.Setup(r => r.GetFavoriteTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        repository.Setup(r => r.GetByTextAsync("Quote A", It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainQuot?)null);
        repository.Setup(r => r.GetByTextAsync("Quote B", It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainQuot?)null);
        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(seeds);
        var catalogLookup = new Mock<IQuoteCatalogLookup>();
        catalogLookup.Setup(l => l.TryGetIndexByTextAsync("Quote A", It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        catalogLookup.Setup(l => l.GetSeedAtAsync(0, It.IsAny<CancellationToken>()))
            .ReturnsAsync(seeds[0]);
        var favoriteStore = new Mock<IFavoriteQuoteTextStore>();
        favoriteStore.Setup(s => s.GetTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal) { "Quote A" });
        favoriteStore.Setup(s => s.SaveTextsAsync(It.IsAny<IReadOnlySet<string>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = QuotServiceTestFactory.Create(repository, provider, catalogLookup, favoriteStore);

        await service.ReseedFeedAsync(1);

        repository.Verify(
            r => r.AddAsync(
                It.Is<DomainQuot>(q => q.Text == "Quote A" && q.IsFavourite),
                It.IsAny<CancellationToken>()),
            Times.Once);
        repository.Verify(
            r => r.AddAsync(
                It.Is<DomainQuot>(q => q.Text == "Quote B" && q.IsFavourite),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task EnsureThemedQuotesInFeedAsync_AddsThemedQuoteFromCatalogWhenMissing()
    {
        IReadOnlyList<QuotSeed> seeds =
        [
            new QuotSeed("Seneca", "Calm quote", "calm"),
            new QuotSeed("Seneca", "Random quote", "wisdom"),
        ];

        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetExistingTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        repository.SetupSequence(r => r.GetUnreadByThemesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<DomainQuot>())
            .ReturnsAsync(Array.Empty<DomainQuot>());
        repository.Setup(r => r.GetReadByThemesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<DomainQuot>());

        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(seeds);

        var service = QuotServiceTestFactory.Create(repository, provider);

        await service.EnsureThemedQuotesInFeedAsync(["calm", "anxiety"], 1);

        repository.Verify(
            r => r.AddManyAsync(
                It.Is<IReadOnlyList<DomainQuot>>(items =>
                    items.Count == 1 &&
                    items[0].Text == "Calm quote" &&
                    items[0].Theme == "calm"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EnsureThemedQuotesInFeedAsync_RestoresReadThemedQuotesWhenCatalogExhausted()
    {
        DomainQuot readQuote = DomainQuot.Create("Seneca", "Calm quote", "calm", isReaded: true, isFavourite: false);

        var repository = new Mock<IQuotRepository>();
        repository.SetupSequence(r => r.GetUnreadByThemesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<DomainQuot>())
            .ReturnsAsync(Array.Empty<DomainQuot>());
        repository.Setup(r => r.GetExistingTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(["Calm quote"]);
        repository.Setup(r => r.GetReadByThemesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([readQuote]);

        var provider = new Mock<IQuotContentProvider>();
        provider.Setup(p => p.LoadAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new QuotSeed("Seneca", "Calm quote", "calm")]);

        var service = QuotServiceTestFactory.Create(repository, provider);

        await service.EnsureThemedQuotesInFeedAsync(["calm"], 1);

        repository.Verify(
            r => r.EditAsync(
                It.Is<DomainQuot>(q => q.Text == "Calm quote" && !q.IsReaded),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
