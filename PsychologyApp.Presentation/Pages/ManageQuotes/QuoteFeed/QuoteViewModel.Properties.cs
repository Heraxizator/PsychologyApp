using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Shared.UI.Components;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;

public partial class QuoteViewModel
{
    private bool _showAllReadEmpty;
    public bool ShowAllReadEmpty
    {
        get => _showAllReadEmpty;
        private set
        {
            if (SetProperty(ref _showAllReadEmpty, value))
            {
                NotifyEmptyStateProperties();
            }
        }
    }

    private QuoteItem? _dailyQuote;
    public QuoteItem? DailyQuote
    {
        get => _dailyQuote;
        private set
        {
            if (SetProperty(ref _dailyQuote, value))
            {
                OnPropertyChanged(nameof(HasDailyQuote));
                OnPropertyChanged(nameof(ShowDailyQuoteHeader));
                OnPropertyChanged(nameof(DailyQuoteText));
                OnPropertyChanged(nameof(DailyQuoteAuthor));
                OnPropertyChanged(nameof(DailyQuoteLikeCommand));
                OnPropertyChanged(nameof(DailyQuoteCopyCommand));
                OnPropertyChanged(nameof(DailyQuoteShareCommand));
                OnPropertyChanged(nameof(DailyQuoteIsFavourite));
            }
        }
    }

    public bool HasDailyQuote => DailyQuote is not null;

    public bool ShowDailyQuoteHeader =>
        HasDailyQuote &&
        !IsSearching &&
        FeedMode is QuoteFeedMode.All or QuoteFeedMode.ForYou;

    public bool ShowForYouEmpty =>
        FeedMode == QuoteFeedMode.ForYou &&
        IsDone &&
        !IsSearching &&
        DisplayItems.Count == 0 &&
        !ShowAllReadEmpty;

    public bool ShowFavoritesEmpty =>
        FeedMode == QuoteFeedMode.Favorites &&
        IsDone &&
        !IsSearching &&
        DisplayItems.Count == 0;

    public string DailyQuoteText => DailyQuote?.Text ?? string.Empty;

    public string DailyQuoteAuthor => DailyQuote?.Author ?? string.Empty;

    public ICommand? DailyQuoteLikeCommand => DailyQuote?.LikeCommand;

    public ICommand? DailyQuoteCopyCommand => DailyQuote?.CopyCommand;

    public ICommand? DailyQuoteShareCommand => DailyQuote?.ShareCommand;

    public bool DailyQuoteIsFavourite
    {
        get => DailyQuote?.IsFavourite ?? false;
        set
        {
            if (DailyQuote is null)
            {
                return;
            }

            DailyQuote.IsFavourite = value;
            OnPropertyChanged();
        }
    }

    public string SearchQuery
    {
        get => _searchController.Query;
        set => _searchController.Query = value;
    }

    public bool IsSearching => _searchController.IsSearching;

    public bool IsSearchFilteringVisible =>
        IsDone && IsSearching && _searchController.IsSearchInFlight;

    /// <summary>
    /// Hide the feed while a search filter is in flight so stale results are not shown.
    /// </summary>
    public bool IsQuoteListVisible => IsDone && !IsSearchFilteringVisible;

    public bool IsFeedFiltersVisible => !IsSearching;

    public string EmptyTitleText =>
        ShowAllReadEmpty ? AllReadTitle :
        ShowForYouEmpty ? ForYouEmptyTitle :
        ShowFavoritesEmpty ? FavoritesEmptyTitle :
        IsSearching ? SearchEmptyTitle :
        QuotesEmptyTitle;

    public string EmptyBodyText =>
        ShowAllReadEmpty ? AllReadBody :
        ShowForYouEmpty ? ForYouEmptyBody :
        ShowFavoritesEmpty ? FavoritesEmptyBody :
        IsSearching ? SearchEmptyBody :
        QuotesEmptyBody;

    public string EmptyActionText =>
        ShowAllReadEmpty ? ShowFavoritesButtonText :
        ShowForYouEmpty ? string.Empty :
        ShowFavoritesEmpty ? string.Empty :
        IsSearching ? string.Empty :
        QuotesRefreshButton;

    public ICommand? EmptyActionCommand =>
        ShowAllReadEmpty ? ShowFavoritesCommand :
        ShowForYouEmpty ? null :
        ShowFavoritesEmpty ? null :
        IsSearching ? null :
        Reload;

    public string EmptyIconName =>
        ShowAllReadEmpty ? MaterialIconNames.DoneAll :
        ShowForYouEmpty ? MaterialIconNames.AutoAwesome :
        ShowFavoritesEmpty ? MaterialIconNames.Favorite :
        IsSearching ? MaterialIconNames.Search :
        MaterialIconNames.FormatQuote;
}
