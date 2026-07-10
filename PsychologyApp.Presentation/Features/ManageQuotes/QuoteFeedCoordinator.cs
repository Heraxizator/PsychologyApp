using PsychologyApp.Application.Models;
using PsychologyApp.Application.Quot;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Entities.FilterChip;
using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Features.ManageQuotes.Index;
using PsychologyApp.Presentation.Shared.Common;
using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Features.ManageQuotes;

public sealed class QuoteFeedCoordinator
{
    private readonly IUserProgressService _userProgressService;
    private readonly HashSet<string> _knownQuoteTexts = new(StringComparer.Ordinal);
    private QuoteFeedMode _feedMode = QuoteFeedMode.All;
    private string? _selectedThemeKey;

    public QuoteFeedCoordinator(IUserProgressService userProgressService) =>
        _userProgressService = userProgressService;

    public QuoteFeedMode FeedMode => _feedMode;

    public string? SelectedThemeKey => _selectedThemeKey;

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

    public void SetFeedMode(QuoteFeedMode mode) => _feedMode = mode;

    public bool TrySelectTheme(string? key)
    {
        string? normalized = string.IsNullOrWhiteSpace(key) || string.Equals(key, "all", StringComparison.OrdinalIgnoreCase)
            ? null
            : key;

        if (string.Equals(_selectedThemeKey, normalized, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        _selectedThemeKey = normalized;
        return true;
    }

    public void EnsureThemeFilters(
        ObservableCollection<FilterChipTabItem> filters,
        string allThemesLabel)
    {
        IReadOnlyList<(string Key, string Title)> themes = QuoteThemeLabels.GetFilterThemes();
        int expectedCount = themes.Count + 1;

        if (filters.Count != expectedCount)
        {
            filters.Clear();
            filters.Add(new FilterChipTabItem { Key = "all", Title = allThemesLabel });
            foreach ((string key, string title) in themes)
            {
                filters.Add(new FilterChipTabItem { Key = key, Title = title });
            }
        }
        else
        {
            filters[0].Title = allThemesLabel;
            for (int index = 0; index < themes.Count; index++)
            {
                filters[index + 1].Title = themes[index].Title;
            }
        }

        SyncThemeFilterSelection(filters);
    }

    public void SyncThemeFilterSelection(ObservableCollection<FilterChipTabItem> filters)
    {
        string selectedKey = _selectedThemeKey ?? "all";
        foreach (FilterChipTabItem filter in filters)
        {
            filter.IsSelected = string.Equals(filter.Key, selectedKey, StringComparison.OrdinalIgnoreCase);
        }
    }

    public QuoteFeedMode ParseFeedKey(string? key) =>
        key switch
        {
            "favorites" => QuoteFeedMode.Favorites,
            "for-you" => QuoteFeedMode.ForYou,
            _ => QuoteFeedMode.All
        };

    public async Task<IReadOnlyList<QuotDTO>> FetchQuotesAsync(
        IQuotService quotService,
        int count,
        CancellationToken cancellationToken)
    {
        IEnumerable<QuotDTO> quotDTOs = await FetchRawQuotesAsync(quotService, count, cancellationToken);

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

    public bool ShouldSeedNewQuote(bool seedNewQuote) =>
        seedNewQuote && _feedMode is QuoteFeedMode.All or QuoteFeedMode.ForYou;

    public async Task<bool> ShouldShowAllReadEmptyAsync(
        int collectionCount,
        bool isDone,
        IQuotService quotService,
        CancellationToken cancellationToken) =>
        _feedMode == QuoteFeedMode.All &&
        collectionCount == 0 &&
        isDone &&
        await quotService.IsAllCaughtUpAsync(cancellationToken);

    public void EnsureFeedFilters(
        ObservableCollection<FilterChipTabItem> filters,
        string allLabel,
        string favoritesLabel,
        string forYouLabel)
    {
        if (filters.Count == 0)
        {
            filters.Add(new FilterChipTabItem { Key = "all", Title = allLabel });
            filters.Add(new FilterChipTabItem { Key = "for-you", Title = forYouLabel });
            filters.Add(new FilterChipTabItem { Key = "favorites", Title = favoritesLabel });
        }
        else
        {
            filters[0].Title = allLabel;
            filters[1].Title = forYouLabel;
            filters[2].Title = favoritesLabel;
        }

        SyncFeedFilterSelection(filters);
    }

    public void SyncFeedFilterSelection(ObservableCollection<FilterChipTabItem> filters)
    {
        string selectedKey = _feedMode switch
        {
            QuoteFeedMode.Favorites => "favorites",
            QuoteFeedMode.ForYou => "for-you",
            _ => "all"
        };

        foreach (FilterChipTabItem filter in filters)
        {
            filter.IsSelected = filter.Key == selectedKey;
        }
    }

    public bool ShouldExcludeFromFeed(string? text, string? dailyQuoteText) =>
        !string.IsNullOrEmpty(text) &&
        !string.IsNullOrEmpty(dailyQuoteText) &&
        string.Equals(text, dailyQuoteText, StringComparison.Ordinal);

    public async Task<QuoteFeedLoadResult> LoadItemsAsync(
        IQuotService quotService,
        QuoteItemCommandsFactory factory,
        int count,
        bool resetKnown,
        bool seedNewQuote,
        string? dailyQuoteText,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail,
        CancellationToken cancellationToken)
    {
        if (resetKnown)
        {
            ResetKnownQuotes();
        }

        if (ShouldSeedNewQuote(seedNewQuote) && await ShouldSeedBecauseFeedIsEmptyAsync(quotService, cancellationToken))
        {
            await SeedSingleQuoteAsync(quotService, cancellationToken);
        }

        IReadOnlyList<QuoteItem> items = await FetchMappedItemsAsync(
            quotService,
            factory,
            count,
            dailyQuoteText,
            refreshBindingAsync,
            onFail,
            cancellationToken);

        bool allCaughtUp = await ShouldShowAllReadEmptyAsync(
            items.Count,
            isDone: true,
            quotService,
            cancellationToken);

        return new QuoteFeedLoadResult(items, allCaughtUp);
    }

    public async Task<IReadOnlyList<QuoteItem>> AppendItemsAsync(
        IQuotService quotService,
        QuoteItemCommandsFactory factory,
        int count,
        bool seedSingleFirst,
        string? dailyQuoteText,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail,
        CancellationToken cancellationToken)
    {
        if (seedSingleFirst)
        {
            await SeedSingleQuoteAsync(quotService, cancellationToken);
        }

        return await FetchMappedItemsAsync(
            quotService,
            factory,
            count,
            dailyQuoteText,
            refreshBindingAsync,
            onFail,
            cancellationToken);
    }

    private async Task<IReadOnlyList<QuoteItem>> FetchMappedItemsAsync(
        IQuotService quotService,
        QuoteItemCommandsFactory factory,
        int count,
        string? dailyQuoteText,
        Func<QuoteItem, Task> refreshBindingAsync,
        Action onFail,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<QuotDTO> quotDTOs = await FetchQuotesAsync(quotService, count, cancellationToken);
        List<QuoteItem> items = [];
        foreach (QuotDTO quotDTO in quotDTOs)
        {
            if (ShouldExcludeFromFeed(quotDTO.Text, dailyQuoteText))
            {
                continue;
            }

            items.Add(factory.CreateQuoteItem(quotDTO, refreshBindingAsync, onFail));
        }

        return items;
    }

    private async Task<IEnumerable<QuotDTO>> FetchRawQuotesAsync(
        IQuotService quotService,
        int count,
        CancellationToken cancellationToken)
    {
        if (_feedMode == QuoteFeedMode.Favorites)
        {
            IEnumerable<QuotDTO> favorites = await quotService.GetFavouritesAsync(count, cancellationToken);
            IEnumerable<QuotDTO> favouriteItems = favorites.Where(quotDTO => quotDTO.IsFavourite);
            return FilterBySelectedTheme(favouriteItems);
        }

        if (!string.IsNullOrWhiteSpace(_selectedThemeKey))
        {
            await quotService.EnsureThemedQuotesInFeedAsync([_selectedThemeKey], count, cancellationToken);
            return await quotService.GetUnreadByThemesAsync([_selectedThemeKey], count, cancellationToken);
        }

        if (_feedMode == QuoteFeedMode.ForYou)
        {
            int? todayMood = await ResolveTodayMoodLevelAsync(cancellationToken);
            IReadOnlyList<string> themes = QuotePersonalizationPolicy.ResolveThemes(
                UserPreferences.OnboardingConcern,
                todayMood);
            await quotService.EnsureThemedQuotesInFeedAsync(themes, count, cancellationToken);
            return await quotService.GetUnreadByThemesAsync(themes, count, cancellationToken);
        }

        return await quotService.GetUnreadAsync(count, cancellationToken);
    }

    private IEnumerable<QuotDTO> FilterBySelectedTheme(IEnumerable<QuotDTO> quotes)
    {
        if (string.IsNullOrWhiteSpace(_selectedThemeKey))
        {
            return quotes;
        }

        return quotes.Where(quote =>
            string.Equals(quote.Theme, _selectedThemeKey, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<int?> ResolveTodayMoodLevelAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<MoodEntryDTO> moods = await _userProgressService.GetRecentMoodsAsync(1, cancellationToken);
        if (moods.Count == 0 || moods[0].RecordedAt.ToLocalTime().Date != DateTime.Today)
        {
            return null;
        }

        return moods[0].MoodLevel;
    }

    private async Task SeedSingleQuoteAsync(IQuotService quotService, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_selectedThemeKey))
        {
            await quotService.TryLoadThemedSingleAsync([_selectedThemeKey], cancellationToken);
            return;
        }

        if (_feedMode == QuoteFeedMode.ForYou)
        {
            int? todayMood = await ResolveTodayMoodLevelAsync(cancellationToken);
            IReadOnlyList<string> themes = QuotePersonalizationPolicy.ResolveThemes(
                UserPreferences.OnboardingConcern,
                todayMood);
            await quotService.TryLoadThemedSingleAsync(themes, cancellationToken);
            return;
        }

        await quotService.TryLoadSingleAsync(cancellationToken);
    }

    private static async Task<bool> ShouldSeedBecauseFeedIsEmptyAsync(
        IQuotService quotService,
        CancellationToken cancellationToken)
    {
        IEnumerable<QuotDTO> unread =
            await quotService.GetUnreadAsync(1, cancellationToken);
        return !unread.Any();
    }
}

public sealed record QuoteFeedLoadResult(IReadOnlyList<QuoteItem> Items, bool ShowAllCaughtUp);
