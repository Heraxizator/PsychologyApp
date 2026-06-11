using System.ComponentModel;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Physics;

namespace PsychologyApp.Presentation.Views.Physics;

public partial class PhysicsSearchPage : ContentPage
{
    PhysicsSearchViewModel ViewModel { get; set; }
    private PageAnimationHelper? _animationHelper;

    public PhysicsSearchPage(
        IPageViewModelActivator pageViewModelActivator,
        IPhysicsSearchViewModelFactory physicsSearchViewModelFactory)
    {
        InitializeComponent();
        ViewModel = this.ActivateViewModel(pageViewModelActivator, nav => physicsSearchViewModelFactory.Create(nav));
        _animationHelper = new PageAnimationHelper(ViewModel, InitLoadingProgress, SearchResultsCollectionView);
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PhysicsSearchViewModel.IsSearchFilteringVisible))
        {
            UiStateAnimator.AnimateVisibilityAsync(SearchFilteringProgress, ViewModel.IsSearchFilteringVisible).FireAndForget();
        }

        if (e.PropertyName == nameof(PhysicsSearchViewModel.IsSearchEmptyPromptVisible))
        {
            UiStateAnimator.AnimateVisibilityAsync(SearchEmptyPrompt, ViewModel.IsSearchEmptyPromptVisible).FireAndForget();
        }

        if (e.PropertyName == nameof(PhysicsSearchViewModel.IsSearchNoResultsVisible))
        {
            UiStateAnimator.AnimateVisibilityAsync(SearchNoResults, ViewModel.IsSearchNoResultsVisible).FireAndForget();
        }

        if (e.PropertyName == nameof(PhysicsSearchViewModel.IsSearchResultsListVisible) && ViewModel.IsSearchResultsListVisible)
        {
            UiAnimations.SafeRevealPremiumAsync(SearchResultsCollectionView, allowHidden: true).FireAndForget();
        }
    }

    private void OnRemainingItemsThresholdReached(object? sender, EventArgs e) =>
        ViewModel.LoadMoreSearchResultsCommand.Execute(null);

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
