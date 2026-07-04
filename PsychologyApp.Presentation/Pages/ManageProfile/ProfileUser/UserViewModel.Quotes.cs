namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Features.ManageProfile;

public partial class UserViewModel
{
    private bool _isQuotesLoading;
    public bool IsQuotesLoading
    {
        get => _isQuotesLoading;
        private set => SetProperty(ref _isQuotesLoading, value);
    }

    private bool _isQuotesReady;
    public bool IsQuotesReady
    {
        get => _isQuotesReady;
        private set => SetProperty(ref _isQuotesReady, value);
    }

    private bool _isQuotesFailed;
    public bool IsQuotesFailed
    {
        get => _isQuotesFailed;
        private set => SetProperty(ref _isQuotesFailed, value);
    }

    private async Task LoadQuotesAsync(int generation, CancellationToken outerToken)
    {
        await UiThread.RunAsync(() =>
        {
            IsQuotesFailed = false;
            IsQuotesReady = false;
            IsQuotesLoading = true;
            NotifyQuotesPresentation();
        });

        ProfileQuotesLoadResult result = await _profileQuotesLoader.LoadFavoritesAsync(
            count: 5,
            generation,
            () => Volatile.Read(ref _initGeneration),
            outerToken,
            _quoteCommandsFactory,
            RefreshProfileQuoteBindingAsync,
            SetQuotesFailed,
            OpenQuotesTabCommand);

        switch (result.Status)
        {
            case ProfileQuotesLoadStatus.Ready:
                await UiThread.RunAsync(() =>
                {
                    Quotes.Clear();
                    DisplayQuotes.Clear();
                    foreach (QuoteItem item in result.Items)
                    {
                        Quotes.Add(item);
                    }

                    foreach (QuoteItem item in result.Items.Take(2))
                    {
                        DisplayQuotes.Add(item);
                    }

                    SetQuotesReady();
                    NotifyQuotesPresentation();
                });
                break;
            case ProfileQuotesLoadStatus.Failed:
                if (generation == Volatile.Read(ref _initGeneration))
                {
                    await UiThread.RunAsync(SetQuotesFailed);
                    _logger.LogError("Failed to load profile quotes.");
                }

                break;
        }
    }

    private void CancelQuotesLoading()
    {
        ProfileQuotesCancelResult result = _profileQuotesLoader.CancelLoading(IsQuotesLoading);

        if (result.ShouldRestoreReady)
        {
            SetQuotesReady();
            NotifyQuotesPresentation();
        }
        else if (result.ShouldSetReadyWithoutData)
        {
            IsQuotesLoading = false;
            IsQuotesReady = true;
            NotifyQuotesPresentation();
        }
    }

    private void SetQuotesReady()
    {
        IsQuotesFailed = false;
        IsQuotesLoading = false;
        IsQuotesReady = true;
    }

    private void SetQuotesFailed()
    {
        IsQuotesLoading = false;
        IsQuotesReady = false;
        IsQuotesFailed = true;
        NotifyQuotesPresentation();
    }

    private void NotifyQuotesPresentation()
    {
        OnPropertyChanged(nameof(HasQuotes));
        OnPropertyChanged(nameof(ShowQuotesSectionAction));
        OnPropertyChanged(nameof(ShowQuotesEmpty));
        OnPropertyChanged(nameof(ShowQuotesPreview));
        OnPropertyChanged(nameof(HasQuotesSectionSubtitle));
        OnPropertyChanged(nameof(QuotesSectionSubtitle));
        OnPropertyChanged(nameof(QuotesSectionActionText));
    }

    private Task RefreshProfileQuoteBindingAsync(QuoteItem quoteItem) =>
        UiThread.RunAsync(() =>
        {
            ReplaceQuoteItem(Quotes, quoteItem);
            ReplaceQuoteItem(DisplayQuotes, quoteItem);
            NotifyQuotesPresentation();
        });

    private static void ReplaceQuoteItem(ObservableCollection<QuoteItem> collection, QuoteItem quoteItem)
    {
        for (int index = 0; index < collection.Count; index++)
        {
            if (collection[index].Id == quoteItem.Id)
            {
                collection[index] = quoteItem;
                return;
            }
        }
    }
}
