using System.ComponentModel;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

public partial class TechniquesPage : ContentPage
{
    private TechniquesViewModel? _viewModel;
    private PageAnimationHelper? _animationHelper;
    private bool _bannersSynced;

    public TechniquesPage(
        IPageViewModelActivator pageViewModelActivator,
        ITechniquesViewModelFactory techniquesViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, page => techniquesViewModelFactory.Create(page));
        _animationHelper = new PageAnimationHelper(_viewModel, LoadingProgress, TechniquesCollectionView);
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_viewModel is null || !_viewModel.HasInitialized || !_bannersSynced)
        {
            return;
        }

        if (e.PropertyName == nameof(TechniquesViewModel.ShowEngagementNudge))
        {
            UiStateAnimator.AnimateVisibilityAsync(EngagementNudgeBanner, _viewModel.ShowEngagementNudge).FireAndForget();
        }

        if (e.PropertyName == nameof(TechniquesViewModel.HasWeeklyInsight))
        {
            UiStateAnimator.AnimateVisibilityAsync(WeeklyInsightBanner, _viewModel.HasWeeklyInsight).FireAndForget();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationHelper?.TryRevealAsync();
        if (_viewModel is null)
        {
            return;
        }

        if (_viewModel.HasInitialized)
        {
            if (!_bannersSynced)
            {
                SyncBannerVisibility();
                _bannersSynced = true;
            }

            _viewModel.RefreshOnAppearAsync().FireAndForget();
        }
        else
        {
            InitializeAndSyncBannersAsync().FireAndForget();
        }

        _viewModel.TryOpenPendingTechniqueAsync().FireAndForget();
    }

    private async Task InitializeAndSyncBannersAsync()
    {
        if (_viewModel is null)
        {
            return;
        }

        await _viewModel.EnsureInitializedAsync();
        SyncBannerVisibility();
        _bannersSynced = true;
    }

    private void SyncBannerVisibility()
    {
        if (_viewModel is null)
        {
            return;
        }

        UiStateAnimator.AnimateVisibilityAsync(EngagementNudgeBanner, _viewModel.ShowEngagementNudge).FireAndForget();
        UiStateAnimator.AnimateVisibilityAsync(WeeklyInsightBanner, _viewModel.HasWeeklyInsight).FireAndForget();
    }

    private void OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        if (_viewModel is null)
        {
            return;
        }

        _viewModel.LoadMoreCustomTechniquesCommand.Execute(null);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null)
        {
            if (_viewModel is not null)
            {
                _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            }

            _animationHelper?.Dispose();
            _animationHelper = null;
        }
    }
}
