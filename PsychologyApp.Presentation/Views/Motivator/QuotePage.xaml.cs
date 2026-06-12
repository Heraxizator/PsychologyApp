using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Motivator;

namespace PsychologyApp.Presentation.Views.Motivator;

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

    private async void OnPullToRefresh(object? sender, EventArgs e)
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
