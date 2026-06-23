using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.QuoteFeed;

namespace PsychologyApp.Presentation.Pages.QuoteFeed;

public partial class QuotePage : ContentPage
{
    private QuoteViewModel ViewModel = default!;
    private PageAnimationHelper? _animationHelper;

    public QuotePage(IPageViewModelActivator pageViewModelActivator, IQuoteViewModelFactory quoteViewModelFactory)
    {
        InitializeComponent();
        ViewModel = this.ActivateViewModel(pageViewModelActivator, nav => quoteViewModelFactory.Create(nav));
        _animationHelper = new PageAnimationHelper(ViewModel, LoadingProgress, QuotesCollectionView);
    }

    private void OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        if (!ViewModel.IsDone || ViewModel.IsInit)
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
        _animationHelper?.TryRevealAsync();
        ViewModel.EnsureInitializedAsync().FireAndForget();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null)
        {
            _animationHelper?.Dispose();
            _animationHelper = null;
        }
    }
}
