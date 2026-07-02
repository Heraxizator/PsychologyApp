using System.ComponentModel;
using PsychologyApp.Presentation.Features.SearchPhysics.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.SearchPhysics.PhysicsSearch;

namespace PsychologyApp.Presentation.Pages.SearchPhysics.PhysicsSearch;

public partial class PhysicsSearchPage : ContentPage
{
    PhysicsSearchViewModel ViewModel { get; set; }
    private PageAnimationHelper? _animationHelper;

    public PhysicsSearchPage(
        IPageViewModelActivator pageViewModelActivator,
        IPhysicsSearchViewModelFactory physicsSearchViewModelFactory)
    {
        InitializeComponent();
        ViewModel = this.ActivateViewModel(pageViewModelActivator, page => physicsSearchViewModelFactory.Create(page));
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
    }

    private void OnRemainingItemsThresholdReached(object? sender, EventArgs e) =>
        ViewModel.LoadMoreSearchResultsCommand.Execute(null);

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
