using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice;
using PsychologyApp.Application.Quot;
using PsychologyApp.Application.Startup;
using Xunit;

namespace PsychologyApp.Application.Tests.Startup;

public class AppStartupServiceTests
{
    [Fact]
    public async Task InitializeAsync_WhenQuotePreloadFails_ContinuesWithoutThrowing()
    {
        var databaseInitializer = new Mock<IDatabaseInitializer>();
        var quotService = new Mock<IQuotService>();
        var quoteCatalogVersionStore = new Mock<IQuoteCatalogVersionStore>();
        var techniqueCatalogService = new Mock<ITechniqueCatalogService>();
        quoteCatalogVersionStore
            .Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(QuoteCatalogPolicy.CurrentVersion);
        quotService
            .Setup(s => s.LoadSingleAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("catalog empty"));
        techniqueCatalogService
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<BuiltInTechniqueDefinition>());

        var service = CreateService(
            databaseInitializer.Object,
            quotService.Object,
            quoteCatalogVersionStore.Object,
            techniqueCatalogService.Object);

        await service.InitializeAsync();

        databaseInitializer.Verify(d => d.InitializeAsync(It.IsAny<CancellationToken>()), Times.Once);
        techniqueCatalogService.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_WhenCatalogVersionIsOlder_ReseedsFeed()
    {
        var databaseInitializer = new Mock<IDatabaseInitializer>();
        var quotService = new Mock<IQuotService>();
        var quoteCatalogVersionStore = new Mock<IQuoteCatalogVersionStore>();
        var techniqueCatalogService = new Mock<ITechniqueCatalogService>();
        quoteCatalogVersionStore
            .Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(QuoteCatalogPolicy.CurrentVersion - 1);
        techniqueCatalogService
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<BuiltInTechniqueDefinition>());

        var service = CreateService(
            databaseInitializer.Object,
            quotService.Object,
            quoteCatalogVersionStore.Object,
            techniqueCatalogService.Object);

        await service.InitializeAsync();

        quotService.Verify(
            s => s.ReseedFeedAsync(QuoteCatalogPolicy.DefaultFeedSeedCount, It.IsAny<CancellationToken>()),
            Times.Once);
        quoteCatalogVersionStore.Verify(
            s => s.SetAsync(QuoteCatalogPolicy.CurrentVersion, It.IsAny<CancellationToken>()),
            Times.Once);
        quotService.Verify(s => s.LoadSingleAsync(It.IsAny<CancellationToken>()), Times.Never);
        techniqueCatalogService.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_WhenCatalogVersionIsCurrent_LoadsSingleQuote()
    {
        var databaseInitializer = new Mock<IDatabaseInitializer>();
        var quotService = new Mock<IQuotService>();
        var quoteCatalogVersionStore = new Mock<IQuoteCatalogVersionStore>();
        var techniqueCatalogService = new Mock<ITechniqueCatalogService>();
        quoteCatalogVersionStore
            .Setup(s => s.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(QuoteCatalogPolicy.CurrentVersion);
        techniqueCatalogService
            .Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<BuiltInTechniqueDefinition>());

        var service = CreateService(
            databaseInitializer.Object,
            quotService.Object,
            quoteCatalogVersionStore.Object,
            techniqueCatalogService.Object);

        await service.InitializeAsync();

        quotService.Verify(s => s.LoadSingleAsync(It.IsAny<CancellationToken>()), Times.Once);
        quotService.Verify(
            s => s.ReseedFeedAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
        techniqueCatalogService.Verify(s => s.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static AppStartupService CreateService(
        IDatabaseInitializer databaseInitializer,
        IQuotService quotService,
        IQuoteCatalogVersionStore quoteCatalogVersionStore,
        ITechniqueCatalogService techniqueCatalogService) =>
        new(
            databaseInitializer,
            quotService,
            quoteCatalogVersionStore,
            techniqueCatalogService,
            Options.Create(new AppSettings { MiddleTimeoutMs = 1000 }),
            NullLogger<AppStartupService>.Instance);
}
