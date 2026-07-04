using Moq;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Quot;

namespace PsychologyApp.Application.Tests.Quot;

internal static class QuotServiceTestFactory
{
    public static QuotService Create(
        Mock<IQuotRepository>? repository = null,
        Mock<IQuotContentProvider>? provider = null,
        Mock<IQuoteCatalogLookup>? catalogLookup = null,
        Mock<IFavoriteQuoteTextStore>? favoriteTextStore = null)
    {
        repository ??= new Mock<IQuotRepository>();
        provider ??= new Mock<IQuotContentProvider>();
        catalogLookup ??= new Mock<IQuoteCatalogLookup>();

        if (favoriteTextStore is null)
        {
            favoriteTextStore = new Mock<IFavoriteQuoteTextStore>();
            favoriteTextStore
                .Setup(store => store.GetTextsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HashSet<string>(StringComparer.Ordinal));
            favoriteTextStore
                .Setup(store => store.GetLegacyIndicesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HashSet<int>());
            favoriteTextStore
                .Setup(store => store.SaveTextsAsync(It.IsAny<IReadOnlySet<string>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        repository
            .Setup(r => r.CountAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        repository
            .Setup(r => r.GetFavoriteTextsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());

        return new QuotService(
            repository.Object,
            provider.Object,
            catalogLookup.Object,
            favoriteTextStore.Object);
    }
}
