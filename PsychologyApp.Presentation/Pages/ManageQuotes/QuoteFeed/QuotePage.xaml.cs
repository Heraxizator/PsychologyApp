using PsychologyApp.Presentation.Entities.Quote;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.ManageQuotes;
using PsychologyApp.Presentation.Features.ManageQuotes.DependencyInjection;
using PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;
using System.ComponentModel;

namespace PsychologyApp.Presentation.Pages.ManageQuotes.QuoteFeed;

public partial class QuotePage : ContentPage
{
    private QuoteViewModel ViewModel = default!;
    private PageAnimationHelper? _animationHelper;
    private bool _wasSearching;
    private QuoteFeedMode _lastFeedMode;

    public QuotePage(IPageViewModelActivator pageViewModelActivator, IQuoteViewModelFactory quoteViewModelFactory)
    {
        InitializeComponent();
        ViewModel = this.ActivateViewModel(pageViewModelActivator, page => quoteViewModelFactory.Create(page));
        _animationHelper = new PageAnimationHelper(ViewModel, LoadingProgress, QuotesCollectionView);
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        _wasSearching = ViewModel.IsSearching;
        _lastFeedMode = ViewModel.FeedMode;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(QuoteViewModel.FeedMode) && ViewModel.FeedMode != _lastFeedMode)
        {
            _lastFeedMode = ViewModel.FeedMode;
            UiStateAnimator.CrossfadeContentRefreshAsync(QuotesCollectionView).FireAndForget();
        }

        if (e.PropertyName == nameof(QuoteViewModel.IsSearching) && ViewModel.IsSearching != _wasSearching)
        {
            AnimateSearchModeCrossfadeAsync().FireAndForget();
            _wasSearching = ViewModel.IsSearching;
        }

        if (e.PropertyName == nameof(QuoteViewModel.IsSearchFilteringVisible))
        {
            UiStateAnimator.AnimateVisibilityAsync(SearchFilteringProgress, ViewModel.IsSearchFilteringVisible).FireAndForget();
        }
    }

    private async Task AnimateSearchModeCrossfadeAsync()
    {
        if (ViewModel.IsSearching)
        {
            if (FeedFilterBar.IsVisible)
            {
                await UiAnimations.SafeHideAsync(FeedFilterBar);
            }

            return;
        }

        FeedFilterBar.IsVisible = true;
        await UiAnimations.SafeRevealLiteAsync(
            FeedFilterBar,
            UiAnimations.TabReappearSlideOffset,
            allowHidden: true);
    }

    private void OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        if (!ViewModel.IsDone || ViewModel.IsInit || ViewModel.IsSearching)
        {
            return;
        }

        ViewModel.LoadMoreQuotesCommand.Execute(null);
    }

    private void OnPullToRefresh(object? sender, EventArgs e) =>
        HandlePullToRefreshAsync().FireAndForget();

    private async Task HandlePullToRefreshAsync()
    {
        try
        {
            await ViewModel.ReloadFromPullAsync();
        }
        finally
        {
            QuotesRefresh.IsRefreshing = false;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        HandleOnAppearingAsync().FireAndForget();
    }

    private async Task HandleOnAppearingAsync()
    {
        _animationHelper?.TryRevealAsync();
        await ViewModel.EnsureInitializedAsync();
        await ViewModel.TryApplyPendingFeedAsync();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null)
        {
            ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _animationHelper?.Dispose();
            _animationHelper = null;
        }
    }
}
