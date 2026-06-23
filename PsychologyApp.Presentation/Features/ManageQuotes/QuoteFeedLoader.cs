using PsychologyApp.Application.Models;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Entities.Quote;

namespace PsychologyApp.Presentation.Features.ManageQuotes;

public sealed class QuoteFeedLoader
{
    public async Task<IReadOnlyList<QuoteItem>> LoadItemsAsync(
        QuoteFeedCoordinator coordinator,
        IQuotService quotService,
        QuoteItemCommandsFactory factory,
        int count,
        bool resetKnown,
        bool seedNewQuote,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail,
        CancellationToken cancellationToken)
    {
        if (resetKnown)
        {
            coordinator.ResetKnownQuotes();
        }

        if (coordinator.ShouldSeedNewQuote(seedNewQuote))
        {
            await quotService.LoadSingleAsync(cancellationToken);
        }

        return await FetchMappedItemsAsync(
            coordinator,
            quotService,
            factory,
            count,
            refreshBindingAsync,
            onFail,
            cancellationToken);
    }

    public async Task<IReadOnlyList<QuoteItem>> AppendItemsAsync(
        QuoteFeedCoordinator coordinator,
        IQuotService quotService,
        QuoteItemCommandsFactory factory,
        int count,
        bool seedSingleFirst,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail,
        CancellationToken cancellationToken)
    {
        if (seedSingleFirst)
        {
            await quotService.LoadSingleAsync(cancellationToken);
        }

        return await FetchMappedItemsAsync(
            coordinator,
            quotService,
            factory,
            count,
            refreshBindingAsync,
            onFail,
            cancellationToken);
    }

    private static async Task<IReadOnlyList<QuoteItem>> FetchMappedItemsAsync(
        QuoteFeedCoordinator coordinator,
        IQuotService quotService,
        QuoteItemCommandsFactory factory,
        int count,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<QuotDTO> quotDTOs = await coordinator.FetchQuotesAsync(quotService, count, cancellationToken);
        List<QuoteItem> items = [];
        foreach (QuotDTO quotDTO in quotDTOs)
        {
            items.Add(factory.CreateQuoteItem(quotDTO, refreshBindingAsync, onFail));
        }

        return items;
    }
}
