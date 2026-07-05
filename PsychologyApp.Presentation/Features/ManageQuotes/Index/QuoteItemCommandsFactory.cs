using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Shared.Services.Clipboard;
using PsychologyApp.Presentation.Shared.Services.Toasts;
using PsychologyApp.Presentation.Shared.UI.Overlays;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Features.ManageQuotes.Index;

public sealed class QuoteItemCommandsFactory(
    IQuotService quotService,
    IQuotesChangeNotifier quotesChangeNotifier,
    IAppClipboardService clipboardService,
    IToastService toastService,
    IOptions<AppSettings> settings,
    ILogger<QuoteItemCommandsFactory> logger)
{
    public ICommand CreateShareCommand(string? text, string? author) =>
        new AsyncCommand(() => Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = QuoteShareFormatter.Format(text ?? string.Empty, author ?? AppStrings.UnknownAuthor),
            Title = AppStrings.QuoteShareTitle
        }));

    public ICommand CreateCopyCommand(string text, string author) =>
        new AsyncCommand(() => clipboardService.CopyWithFeedbackAsync(
            QuoteShareFormatter.Format(text, author),
            AppStrings.QuoteCopied,
            AppToastKind.Success));

    public QuoteItem CreateQuoteItem(
        QuotDTO quotDTO,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail,
        bool isDailyQuote = false)
    {
        QuoteItem quoteItem = new()
        {
            Id = quotDTO.QuotId,
            Text = quotDTO.Text!,
            Author = quotDTO.Title!,
            Theme = quotDTO.Theme ?? "general",
            ThemeLabel = QuoteThemeLabels.GetLabel(quotDTO.Theme),
            IsFavourite = quotDTO.IsFavourite,
            IsReaded = quotDTO.IsReaded,
            IsDailyQuote = isDailyQuote,
            ShareCommand = CreateShareCommand(quotDTO.Text, quotDTO.Title)
        };

        quoteItem.LikeCommand = CreateLikeCommand(quoteItem, refreshBindingAsync, onFail);
        quoteItem.CopyCommand = CreateCopyCommand(quoteItem.Text, quoteItem.Author);
        quoteItem.MarkReadCommand = CreateMarkReadCommand(quoteItem);
        return quoteItem;
    }

    public QuoteItem CreateSearchResultItem(
        string author,
        string text,
        string theme,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail)
    {
        QuoteItem quoteItem = new()
        {
            Id = 0,
            Text = text,
            Author = author,
            Theme = theme,
            ThemeLabel = QuoteThemeLabels.GetLabel(theme),
            ShareCommand = CreateShareCommand(text, author)
        };

        quoteItem.CopyCommand = CreateCopyCommand(text, author);
        quoteItem.LikeCommand = CreateLikeCommand(quoteItem, refreshBindingAsync, onFail);
        return quoteItem;
    }

    public ICommand CreateLikeCommand(
        QuoteItem quoteItem,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail) =>
        new AsyncCommand(async () =>
        {
            if (quoteItem.Id <= 0)
            {
                return;
            }

            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(settings);
            await MarkAsFavouriteAsync(
                quoteItem,
                !quoteItem.IsFavourite,
                timeoutSource.Token,
                refreshBindingAsync,
                onFail);
        });

    private ICommand CreateMarkReadCommand(QuoteItem quoteItem) =>
        new AsyncCommand(async () =>
        {
            if (quoteItem.Id <= 0 || quoteItem.IsReaded)
            {
                return;
            }

            try
            {
                using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(settings);
                await quotService.MarkAsReadedAsync(quoteItem.Id, timeoutSource.Token);
                quoteItem.IsReaded = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to mark quote as read.");
            }
        });

    public async Task MarkAsFavouriteAsync(
        QuoteItem quoteItem,
        bool isFavourite,
        CancellationToken cancellationToken,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail)
    {
        bool previousValue = quoteItem.IsFavourite;

        try
        {
            quoteItem.IsFavourite = isFavourite;
            await refreshBindingAsync(quoteItem);
            await quotService.MarkAsFavouriteAsync(quoteItem.Id, isFavourite, cancellationToken);
            quotesChangeNotifier.NotifyFavoritesChanged();
            toastService.ShortToast(isFavourite
                ? AppStrings.QuotesFavoriteAdded
                : AppStrings.QuotesFavoriteRemoved,
                AppToastKind.Success);
        }
        catch (Exception ex)
        {
            quoteItem.IsFavourite = previousValue;
            await refreshBindingAsync(quoteItem);
            onFail();
            logger.LogError(ex, "Failed to mark quote as favourite.");
        }
    }
}
