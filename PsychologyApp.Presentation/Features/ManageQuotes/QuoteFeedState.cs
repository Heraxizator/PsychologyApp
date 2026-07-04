using MvvmHelpers;
using PsychologyApp.Presentation.Entities.Quote;

namespace PsychologyApp.Presentation.Features.ManageQuotes;

public sealed class QuoteFeedState
{
    public ObservableRangeCollection<QuoteItem> DisplayItems { get; } = [];

    private List<QuoteItem> _feedItems = [];

    public int FeedItemCount => _feedItems.Count;

    public void ClearFeed()
    {
        _feedItems.Clear();
        DisplayItems.Clear();
    }

    public void SetFeedItems(IReadOnlyList<QuoteItem> items)
    {
        _feedItems = items.ToList();
        DisplayItems.ReplaceRange(_feedItems);
    }

    public void RestoreFeedDisplay() => DisplayItems.ReplaceRange(_feedItems);

    public void AppendItems(IEnumerable<QuoteItem> items)
    {
        foreach (QuoteItem item in items)
        {
            _feedItems.Add(item);
            DisplayItems.Add(item);
        }
    }

    public bool TryUpdateFeedItem(QuoteItem quoteItem, bool isSearching)
    {
        int index = _feedItems.IndexOf(quoteItem);
        if (index < 0)
        {
            return false;
        }

        _feedItems[index] = quoteItem;
        if (!isSearching)
        {
            int displayIndex = DisplayItems.IndexOf(quoteItem);
            if (displayIndex >= 0)
            {
                DisplayItems[displayIndex] = quoteItem;
            }
        }

        return true;
    }
}
