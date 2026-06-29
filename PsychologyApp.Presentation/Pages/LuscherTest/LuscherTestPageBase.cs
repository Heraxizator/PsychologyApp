using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Pages.LuscherTest;

public abstract class LuscherTestPageBase : ContentPage
{
    private LuscherTestViewModel? _viewModel;

    protected LuscherTestPageBase()
    {
        Loaded += OnPageLoaded;
    }

    protected void InitializeLuscherViewModel(
        LuscherMode mode,
        IPageViewModelActivator pageViewModelActivator,
        ILuscherTestViewModelFactory luscherTestViewModelFactory)
    {
        _viewModel = this.ActivateViewModel(
            pageViewModelActivator,
            page => luscherTestViewModelFactory.Create(page, mode));
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected abstract View LuscherStartSection { get; }
    protected abstract View LuscherFinishSection { get; }

    private void OnPageLoaded(object? sender, EventArgs e)
    {
        foreach (VisualElement element in this.GetVisualTreeDescendants())
        {
            if (element is not View view)
            {
                continue;
            }

            if (view.GestureRecognizers.OfType<TapGestureRecognizer>().Any() &&
                element is Border or BoxView)
            {
                VisualElementPressFeedback.Attach(element);
            }
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (_viewModel is null || !IsLoaded)
        {
            return;
        }

        if (LuscherStartSection is null || LuscherFinishSection is null)
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
