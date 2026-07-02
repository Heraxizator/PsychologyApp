using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Quote;

namespace PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;

public partial class QuoteViewModel
{
    private async Task<bool> InitAsync(bool seedNewQuote)
    {
        try
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateMiddleTimeoutSource(_settings);
            CancellationToken cancellationToken = timeoutSource.Token;

            await _databaseReadySignal.WaitAsync(cancellationToken);
            await UiThread.RunAsync(SetInit);

            IReadOnlyList<QuoteItem> items = await _quoteFeedLoader.LoadItemsAsync(
                _feedCoordinator,
                _quotService,
                _quoteCommandsFactory,
                count: 20,
                resetKnown: true,
                seedNewQuote,
                RefreshQuoteBindingAsync,
                SetFail,
                cancellationToken);

            await UiThread.RunAsync(() =>
            {
                QuotesObservableCollection.Clear();
                foreach (QuoteItem item in items)
                {
                    QuotesObservableCollection.Add(item);
                }

                UpdateAllReadEmptyState();
                SetDone();
            });

            return true;
        }
        catch (Exception e)
        {
            await UiThread.RunAsync(SetFail);
            _logger.LogError(e, "QuoteViewModel init failed.");
            return false;
        }
    }

    private Task RefreshQuoteBindingAsync(QuoteItem quoteItem)
    {
        int index = QuotesObservableCollection.IndexOf(quoteItem);
        if (index < 0)
        {
            return Task.CompletedTask;
        }

        return UiThread.RunAsync(() => QuotesObservableCollection[index] = quoteItem);
    }

    public async Task AddFreshQuotesAsync(CancellationToken cancellationToken = default)
    {
        using CancellationTokenSource? timeoutSource = cancellationToken.CanBeCanceled
            ? null
            : OperationCancellation.CreateSmallTimeoutSource(_settings);
        CancellationToken effectiveToken = timeoutSource?.Token ?? cancellationToken;

        try
        {
            IReadOnlyList<QuoteItem> items = await _quoteFeedLoader.AppendItemsAsync(
                _feedCoordinator,
                _quotService,
                _quoteCommandsFactory,
                count: 5,
                seedSingleFirst: _feedCoordinator.FeedMode == QuoteFeedMode.All,
                RefreshQuoteBindingAsync,
                SetFail,
                effectiveToken);

            await UiThread.RunAsync(() =>
            {
                foreach (QuoteItem item in items)
                {
                    QuotesObservableCollection.Add(item);
                }

                UpdateAllReadEmptyState();
            });
        }
        catch (Exception e)
        {
            await UiThread.RunAsync(SetFail);
            _logger.LogError(e, "Failed to add fresh quotes.");
        }
    }
}
