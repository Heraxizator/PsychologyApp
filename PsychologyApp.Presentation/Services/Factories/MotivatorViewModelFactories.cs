using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Presentation.Services.Toasts;
using PsychologyApp.Presentation.ViewModels.Motivator;
using PsychologyApp.Presentation.ViewModels.Clean;

namespace PsychologyApp.Presentation.Services.Factories;

public interface IQuoteViewModelFactory
{
    QuoteViewModel Create(INavigation navigation);
}

public sealed class QuoteViewModelFactory(
    IQuotService quotService,
    ILogger<QuoteViewModel> logger,
    IOptions<AppSettings> settings,
    IToastService toastService) : IQuoteViewModelFactory
{
    public QuoteViewModel Create(INavigation navigation) =>
        new(navigation, quotService, logger, settings, toastService);
}

public interface IMusicPlayerViewModelFactory
{
    MusicPlayerViewModel Create();
}

public sealed class MusicPlayerViewModelFactory(ILogger<MusicPlayerViewModel> logger) : IMusicPlayerViewModelFactory
{
    public MusicPlayerViewModel Create() => new(logger);
}
