using System.ComponentModel;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Tests;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class AlternativeTestPage : ContentPage
{
    private AlternativeTestViewModel? _viewModel;

    public AlternativeTestPage(
        IPageViewModelActivator pageViewModelActivator,
        IAlternativeTestViewModelFactory alternativeTestViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, nav => alternativeTestViewModelFactory.Create(nav));
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        Loaded += OnPageLoaded;
    }

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

        if (e.PropertyName == nameof(AlternativeTestViewModel.IsFinish) && _viewModel.IsFinish)
        {
            UiStateAnimator.CrossfadeSectionsAsync(StartSection, FinishSection).FireAndForget();
        }
        else if (e.PropertyName == nameof(AlternativeTestViewModel.IsStart) && _viewModel.IsStart)
        {
            UiStateAnimator.CrossfadeSectionsAsync(FinishSection, StartSection).FireAndForget();
        }
    }
}
