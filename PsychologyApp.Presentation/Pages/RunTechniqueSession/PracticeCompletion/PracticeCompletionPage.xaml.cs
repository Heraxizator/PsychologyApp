using System.ComponentModel;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.PracticeCompletion;

public partial class PracticeCompletionPage : ContentPage
{
    private PracticeCompletionViewModel? _viewModel;
    private bool _motionSynced;
    private bool _hadMoodDelta;

    public PracticeCompletionPage(IPracticeCompletionViewModelFactory factory, int streakDays, string? completedItemKey = null)
    {
        _viewModel = factory.Create(this, streakDays, completedItemKey);
        BindingContext = _viewModel;
        InitializeComponent();
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel is null)
        {
            return;
        }

        if (!_motionSynced)
        {
            SyncMotionVisibility(animate: false);
            _motionSynced = true;
            _hadMoodDelta = _viewModel.HasMoodDelta;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_viewModel is null || !_motionSynced)
        {
            return;
        }

        if (e.PropertyName == nameof(PracticeCompletionViewModel.HasNextPractice))
        {
            UiStateAnimator.AnimateVisibilityAsync(NextPracticeRow, _viewModel.HasNextPractice).FireAndForget();
        }

        if (e.PropertyName is nameof(PracticeCompletionViewModel.HasMoodDelta)
            or nameof(PracticeCompletionViewModel.SelectedMoodLevel)
            or nameof(PracticeCompletionViewModel.BeforeMoodLevel))
        {
            HandleMoodDeltaMotionAsync().FireAndForget();
        }
    }

    private async Task HandleMoodDeltaMotionAsync()
    {
        if (_viewModel is null)
        {
            return;
        }

        bool hasMoodDelta = _viewModel.HasMoodDelta;
        await UiStateAnimator.AnimateVisibilityAsync(MoodDeltaLabel, hasMoodDelta);

        if (hasMoodDelta && !_hadMoodDelta
            && _viewModel.SelectedMoodLevel > _viewModel.BeforeMoodLevel)
        {
            await UiAnimations.SafePulseAsync(MoodDeltaLabel);
        }

        _hadMoodDelta = hasMoodDelta;
    }

    private void SyncMotionVisibility(bool animate)
    {
        if (_viewModel is null)
        {
            return;
        }

        if (animate)
        {
            UiStateAnimator.AnimateVisibilityAsync(NextPracticeRow, _viewModel.HasNextPractice).FireAndForget();
            UiStateAnimator.AnimateVisibilityAsync(MoodDeltaLabel, _viewModel.HasMoodDelta).FireAndForget();
            return;
        }

        NextPracticeRow.IsVisible = _viewModel.HasNextPractice;
        MoodDeltaLabel.IsVisible = _viewModel.HasMoodDelta;

        if (!_viewModel.HasNextPractice)
        {
            UiAnimations.ResetVisualState(NextPracticeRow);
        }

        if (!_viewModel.HasMoodDelta)
        {
            UiAnimations.ResetVisualState(MoodDeltaLabel);
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
