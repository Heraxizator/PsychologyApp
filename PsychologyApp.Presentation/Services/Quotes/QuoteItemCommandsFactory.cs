using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Common.Infrastructure;
using PsychologyApp.Presentation.Models.Profile;
using PsychologyApp.Presentation.Services.Quotes;
using PsychologyApp.Presentation.Services.Toasts;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Services.Quotes;

public sealed class QuoteItemCommandsFactory(
    IQuotService quotService,
    IQuotesChangeNotifier quotesChangeNotifier,
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
        new AsyncCommand(async () =>
        {
            await Clipboard.Default.SetTextAsync(QuoteShareFormatter.Format(text, author));
            toastService.ShortToast(AppStrings.QuoteCopied);
        });

    public QuoteItem CreateQuoteItem(
        QuotDTO quotDTO,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail)
    {
        QuoteItem quoteItem = new()
        {
            Id = quotDTO.QuotId,
            Text = quotDTO.Text!,
            Author = quotDTO.Title!,
            IsFavourite = quotDTO.IsFavourite,
            IsReaded = quotDTO.IsReaded,
            ShareCommand = CreateShareCommand(quotDTO.Text, quotDTO.Title)
        };

        quoteItem.LikeCommand = CreateLikeCommand(quoteItem, refreshBindingAsync, onFail);
        quoteItem.CopyCommand = CreateCopyCommand(quoteItem.Text, quoteItem.Author);
        return quoteItem;
    }

    public ICommand CreateLikeCommand(
        QuoteItem quoteItem,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail) =>
        new AsyncCommand(async () =>
        {
            using CancellationTokenSource timeoutSource = OperationCancellation.CreateSmallTimeoutSource(settings);
            await MarkAsFavouriteAsync(
                quoteItem,
                !quoteItem.IsFavourite,
                timeoutSource.Token,
                refreshBindingAsync,
                onFail);
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
                : AppStrings.QuotesFavoriteRemoved);
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
