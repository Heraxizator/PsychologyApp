namespace PsychologyApp.Presentation.Pages.ProfileUser;

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
        });

        ProfileQuotesLoadResult result = await _profileQuotesLoader.LoadFavoritesAsync(
            count: 5,
            generation,
            () => Volatile.Read(ref _initGeneration),
            outerToken,
            OpenQuotesTabCommand,
            _quoteCommandsFactory.CreateShareCommand,
            _quoteCommandsFactory.CreateCopyCommand);

        switch (result.Status)
        {
            case ProfileQuotesLoadStatus.Ready:
                await UiThread.RunAsync(() =>
                {
                    Quotes.Clear();
                    foreach (QuoteItem item in result.Items)
                    {
                        Quotes.Add(item);
                    }

                    OnPropertyChanged(nameof(HasQuotes));
                    OnPropertyChanged(nameof(ShowQuotesEmptyCta));
                    SetQuotesReady();
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
        }
        else if (result.ShouldSetReadyWithoutData)
        {
            IsQuotesLoading = false;
            IsQuotesReady = true;
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
    }
}
