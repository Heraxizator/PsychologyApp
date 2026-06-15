using PsychologyApp.Application;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.QuotService;
using PsychologyApp.Presentation.Models.Common;
using PsychologyApp.Presentation.Models.Profile;
using PsychologyApp.Presentation.ViewModels.Motivator;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Services.Quotes;

public sealed class QuoteFeedCoordinator
{
    private readonly HashSet<string> _knownQuoteTexts = new(StringComparer.Ordinal);
    private QuoteFeedMode _feedMode = QuoteFeedMode.All;

    public QuoteFeedMode FeedMode => _feedMode;

    public void ResetKnownQuotes() => _knownQuoteTexts.Clear();

    public bool TrySwitchFeed(QuoteFeedMode mode)
    {
        if (_feedMode == mode)
        {
            return false;
        }

        _feedMode = mode;
        return true;
    }

    public QuoteFeedMode ParseFeedKey(string? key) =>
        key == "favorites" ? QuoteFeedMode.Favorites : QuoteFeedMode.All;

    public async Task<IReadOnlyList<QuotDTO>> FetchQuotesAsync(
        IQuotService quotService,
        int count,
        CancellationToken cancellationToken)
    {
        IEnumerable<QuotDTO> quotDTOs = _feedMode == QuoteFeedMode.Favorites
            ? await quotService.GetFavouritesAsync(count, cancellationToken)
            : await quotService.GetAllAsync(count, cancellationToken);

        List<QuotDTO> result = [];
        foreach (QuotDTO quotDTO in quotDTOs)
        {
            if (string.IsNullOrEmpty(quotDTO.Text) || !_knownQuoteTexts.Add(quotDTO.Text))
            {
                continue;
            }

            result.Add(quotDTO);
        }

        return result;
    }

    public bool ShouldSeedNewQuote(bool seedNewQuote) => seedNewQuote && _feedMode == QuoteFeedMode.All;

    public bool ShouldShowAllReadEmpty(int collectionCount, bool isDone) =>
        _feedMode == QuoteFeedMode.All && collectionCount == 0 && isDone;

    public void EnsureFeedFilters(
        ObservableCollection<FilterChipTabItem> filters,
        string allLabel,
        string favoritesLabel)
    {
        if (filters.Count == 0)
        {
            filters.Add(new FilterChipTabItem { Key = "all", Title = allLabel });
            filters.Add(new FilterChipTabItem { Key = "favorites", Title = favoritesLabel });
        }
        else
        {
            filters[0].Title = allLabel;
            filters[1].Title = favoritesLabel;
        }

        SyncFeedFilterSelection(filters);
    }

    public void SyncFeedFilterSelection(ObservableCollection<FilterChipTabItem> filters)
    {
        foreach (FilterChipTabItem filter in filters)
        {
            filter.IsSelected = _feedMode == QuoteFeedMode.Favorites
                ? filter.Key == "favorites"
                : filter.Key == "all";
        }
    }
}
