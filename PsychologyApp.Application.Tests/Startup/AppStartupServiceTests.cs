using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PsychologyApp.Application.Abstractions.Persistence;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Exceptions;
using PsychologyApp.Application.Services.QuotService;
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
        quotService
            .Setup(s => s.LoadSingleAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new QuotApiLoadException("offline"));

        var service = new AppStartupService(
            databaseInitializer.Object,
            quotService.Object,
            Options.Create(new AppSettings { MiddleTimeoutMs = 1000 }),
            NullLogger<AppStartupService>.Instance);

        await service.InitializeAsync();

        databaseInitializer.Verify(d => d.InitializeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
