using Moq;
using PsychologyApp.Application.Abstractions.Integration;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Services.QuotService;
using DomainQuot = PsychologyApp.Domain.Entities.Quot;
using Xunit;

namespace PsychologyApp.Application.Tests.Quot;

public class QuotServiceTests
{
    [Fact]
    public async Task LoadSingleAsync_FetchesAndPersistsQuot()
    {
        var quot = DomainQuot.Create("Author", "Text", "Theme", false, false);
        var repository = new Mock<IQuotRepository>();
        var api = new Mock<IQuotApiClient>();
        api.Setup(a => a.FetchRandomQuotAsync(It.IsAny<CancellationToken>())).ReturnsAsync(quot);

        var service = new QuotService(repository.Object, api.Object);

        await service.LoadSingleAsync();

        api.Verify(a => a.FetchRandomQuotAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.AddAsync(quot, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsFavouriteAsync_UpdatesQuot()
    {
        var quot = DomainQuot.Create("Author", "Text", "Theme", false, false);
        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quot);
        repository.Setup(r => r.EditAsync(quot, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var service = new QuotService(repository.Object, Mock.Of<IQuotApiClient>());

        await service.MarkAsFavouriteAsync(1, true);

        repository.Verify(r => r.EditAsync(It.Is<DomainQuot>(q => q.IsFavourite), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsFavouriteAsync_WhenEditFails_ThrowsNotFound()
    {
        var quot = DomainQuot.Create("Author", "Text", "Theme", false, false);
        var repository = new Mock<IQuotRepository>();
        repository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(quot);
        repository.Setup(r => r.EditAsync(quot, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PsychologyApp.Application.Exceptions.PersistenceException("not updated"));

        var service = new QuotService(repository.Object, Mock.Of<IQuotApiClient>());

        await Assert.ThrowsAsync<PsychologyApp.Application.Exceptions.PersistenceException>(
            () => service.MarkAsFavouriteAsync(1, true));
    }
}
