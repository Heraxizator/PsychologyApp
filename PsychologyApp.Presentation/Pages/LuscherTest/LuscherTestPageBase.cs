using System.ComponentModel;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.TestsList;

namespace PsychologyApp.Presentation.Pages.LuscherTest;

public abstract class LuscherTestPageBase : ContentPage
{
    private LuscherTestViewModel? _viewModel;

    protected LuscherTestPageBase(
        LuscherMode mode,
        IPageViewModelActivator pageViewModelActivator,
        ILuscherTestViewModelFactory luscherTestViewModelFactory)
    {
        _viewModel = this.ActivateViewModel(
            pageViewModelActivator,
            nav => luscherTestViewModelFactory.Create(nav, mode));
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        Loaded += OnPageLoaded;
    }

    protected abstract View LuscherStartSection { get; }
    protected abstract View LuscherFinishSection { get; }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        foreach (BoxView boxView in this.GetVisualTreeDescendants().OfType<BoxView>())
        {
            if (boxView.GestureRecognizers.OfType<TapGestureRecognizer>().Any())
            {
                VisualElementPressFeedback.Attach(boxView);
            }
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_viewModel is null)
        {
            return;
        }

        if (e.PropertyName == nameof(LuscherTestViewModel.IsFinish) && _viewModel.IsFinish)
        {
            UiStateAnimator.CrossfadeSectionsAsync(LuscherStartSection, LuscherFinishSection).FireAndForget();
        }
        else if (e.PropertyName == nameof(LuscherTestViewModel.IsStart) && _viewModel.IsStart)
        {
            UiStateAnimator.CrossfadeSectionsAsync(LuscherFinishSection, LuscherStartSection).FireAndForget();
        }
    }
}
