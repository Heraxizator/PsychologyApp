using System.ComponentModel;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Features.RunTests.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Pages.RunTests.TestResult;

public partial class TestResultPage : ContentPage
{
    private TestResultViewModel? _viewModel;
    private bool _motionSynced;

    public TestResultPage(ITestResultViewModelFactory factory, TestResultInfo result)
    {
        InitializeComponent();
        _viewModel = factory.Create(this, result);
        BindingContext = _viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel is null || _motionSynced)
        {
            return;
        }

        SyncMotionVisibility(animate: false);
        _motionSynced = true;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_viewModel is null || !_motionSynced)
        {
            return;
        }

        if (e.PropertyName == nameof(TestResultViewModel.HasTrendChart))
        {
            UiStateAnimator.AnimateVisibilityAsync(TrendChartView, _viewModel.HasTrendChart).FireAndForget();
        }

        if (e.PropertyName == nameof(TestResultViewModel.HasRecommendation))
        {
            UiStateAnimator.AnimateVisibilityAsync(RecommendationSection, _viewModel.HasRecommendation).FireAndForget();
            UiStateAnimator.AnimateVisibilityAsync(RecommendationPrimaryButton, _viewModel.HasRecommendation).FireAndForget();
            UiStateAnimator.AnimateVisibilityAsync(ExplorePracticeButton, _viewModel.ShowExplorePractice).FireAndForget();
            UiStateAnimator.AnimateVisibilityAsync(ExplorePracticeButton, _viewModel.ShowExplorePractice).FireAndForget();
        }

        if (e.PropertyName == nameof(TestResultViewModel.ShowExplorePractice))
        {
            UiStateAnimator.AnimateVisibilityAsync(ExplorePracticeButton, _viewModel.ShowExplorePractice).FireAndForget();
        }
    }

    private void SyncMotionVisibility(bool animate)
    {
        if (_viewModel is null)
        {
            return;
        }

        if (animate)
        {
            UiStateAnimator.AnimateVisibilityAsync(TrendChartView, _viewModel.HasTrendChart).FireAndForget();
            UiStateAnimator.AnimateVisibilityAsync(RecommendationSection, _viewModel.HasRecommendation).FireAndForget();
            UiStateAnimator.AnimateVisibilityAsync(RecommendationPrimaryButton, _viewModel.HasRecommendation).FireAndForget();
            UiStateAnimator.AnimateVisibilityAsync(ExplorePracticeButton, _viewModel.ShowExplorePractice).FireAndForget();
            return;
        }

        TrendChartView.IsVisible = _viewModel.HasTrendChart;
        RecommendationSection.IsVisible = _viewModel.HasRecommendation;
        RecommendationPrimaryButton.IsVisible = _viewModel.HasRecommendation;
        ExplorePracticeButton.IsVisible = _viewModel.ShowExplorePractice;

        if (!_viewModel.HasTrendChart)
        {
            UiAnimations.ResetVisualState(TrendChartView);
        }

        if (!_viewModel.HasRecommendation)
        {
            UiAnimations.ResetVisualState(RecommendationSection);
            UiAnimations.ResetVisualState(RecommendationPrimaryButton);
        }

        if (!_viewModel.ShowExplorePractice)
        {
            UiAnimations.ResetVisualState(ExplorePracticeButton);
        }
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null && _viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _viewModel = null;
        }
    }
}
