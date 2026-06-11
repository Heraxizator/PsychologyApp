using System.ComponentModel;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Tests;

namespace PsychologyApp.Presentation.Views.Tests;

public partial class StandardTestPage : ContentPage
{
    private LuscherTestViewModel? _viewModel;

    public StandardTestPage(
        IPageViewModelActivator pageViewModelActivator,
        ILuscherTestViewModelFactory luscherTestViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(
            pageViewModelActivator,
            nav => luscherTestViewModelFactory.Create(nav, LuscherMode.Standard));
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

        if (e.PropertyName == nameof(LuscherTestViewModel.IsFinish) && _viewModel.IsFinish)
        {
            UiStateAnimator.CrossfadeSectionsAsync(StartSection, FinishSection).FireAndForget();
        }
        else if (e.PropertyName == nameof(LuscherTestViewModel.IsStart) && _viewModel.IsStart)
        {
            UiStateAnimator.CrossfadeSectionsAsync(FinishSection, StartSection).FireAndForget();
        }
    }
}
