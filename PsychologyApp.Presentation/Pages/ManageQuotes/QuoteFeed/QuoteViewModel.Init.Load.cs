using Microsoft.Extensions.Logging;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Quote;

namespace PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;

public partial class QuoteViewModel
{
    private async Task<bool> LoadFeedAsync(bool seedNewQuote, bool isInitialLoad, int generation)
    {
        _feedLoadCts?.Cancel();
        _feedLoadCts?.Dispose();
        _feedLoadCts = OperationCancellation.CreateMiddleTimeoutSource(_settings);
        CancellationToken cancellationToken = _feedLoadCts.Token;

        try
        {
            if (isInitialLoad)
            {
                await _databaseReadySignal.WaitAsync(cancellationToken);
                await UiThread.RunAsync(SetInit);
            }

            await LoadDailyQuoteAsync(cancellationToken);
            if (generation != _feedLoadGeneration)
            {
                return true;
            }

            QuoteFeedLoadResult loadResult = await _feedCoordinator.LoadItemsAsync(
                _quotService,
                _quoteCommandsFactory,
                count: 20,
                resetKnown: true,
                seedNewQuote,
                DailyQuote?.Text,
                RefreshQuoteBindingAsync,
                SetFail,
                cancellationToken);

            if (generation != _feedLoadGeneration)
            {
                return true;
            }

            await UiThread.RunAsync(() =>
            {
                if (generation != _feedLoadGeneration)
                {
                    return;
                }

                _feedState.SetFeedItems(loadResult.Items);
                ShowAllReadEmpty = loadResult.ShowAllCaughtUp;
                SetDone();
                NotifySearchRelatedProperties();
            });

            return true;
        }
        catch (OperationCanceledException)
        {
            if (generation == _feedLoadGeneration)
            {
                await UiThread.RunAsync(CancelProgress);
            }

            return false;
        }
        catch (Exception e)
        {
            if (generation == _feedLoadGeneration)
            {
                await UiThread.RunAsync(SetFail);
            }

            _logger.LogError(e, isInitialLoad ? "QuoteViewModel init failed." : "QuoteViewModel feed reload failed.");
            return false;
        }
    }

    private async Task LoadDailyQuoteAsync(CancellationToken cancellationToken)
    {
        QuotDTO? daily = await _quotService.GetDailyQuoteAsync(DateOnly.FromDateTime(DateTime.Now), cancellationToken);
        if (daily is not { Text: not null and not "" } dailyDto)
        {
            await UiThread.RunAsync(() => DailyQuote = null);
            return;
        }

        QuoteItem item = _quoteCommandsFactory.CreateQuoteItem(
            dailyDto,
            RefreshDailyQuoteBindingAsync,
            SetFail,
            isDailyQuote: true);

        await UiThread.RunAsync(() => DailyQuote = item);
    }

    private Task RefreshQuoteBindingAsync(QuoteItem quoteItem) =>
        UiThread.RunAsync(() => _feedState.TryUpdateFeedItem(quoteItem, IsSearching));

    private Task RefreshDailyQuoteBindingAsync(QuoteItem quoteItem) =>
        UiThread.RunAsync(() =>
        {
            DailyQuote = quoteItem;
            OnPropertyChanged(nameof(DailyQuoteIsFavourite));
        });

    public async Task AddFreshQuotesAsync(CancellationToken cancellationToken = default)
    {
        if (IsSearching)
        {
            return;
        }

        using CancellationTokenSource? timeoutSource = cancellationToken.CanBeCanceled
            ? null
            : OperationCancellation.CreateSmallTimeoutSource(_settings);
        CancellationToken effectiveToken = timeoutSource?.Token ?? cancellationToken;

        try
        {
            IReadOnlyList<QuoteItem> items = await _feedCoordinator.AppendItemsAsync(
                _quotService,
                _quoteCommandsFactory,
                count: 5,
                seedSingleFirst: _feedCoordinator.FeedMode is QuoteFeedMode.All or QuoteFeedMode.ForYou,
                DailyQuote?.Text,
                RefreshQuoteBindingAsync,
                SetFail,
                effectiveToken);

            await UiThread.RunAsync(() => _feedState.AppendItems(items));

            bool allReadEmpty = await _feedCoordinator.ShouldShowAllReadEmptyAsync(
                _feedState.FeedItemCount,
                IsDone,
                _quotService,
                effectiveToken);

            await UiThread.RunAsync(() => ShowAllReadEmpty = allReadEmpty);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            await UiThread.RunAsync(SetFail);
            _logger.LogError(e, "Failed to add fresh quotes.");
        }
    }
}
