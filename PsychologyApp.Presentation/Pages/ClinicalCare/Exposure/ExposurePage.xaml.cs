using System.ComponentModel;
using PsychologyApp.Presentation.Features.ClinicalCare;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.Exposure;

public partial class ExposurePage : ContentPage
{
    public ExposurePage(IExposureViewModelFactory viewModelFactory)
    {
        InitializeComponent();
        ExposureViewModel viewModel = viewModelFactory.Create(this);
        viewModel.PropertyChanged += OnViewModelPropertyChanged;
        BindingContext = viewModel;
    }

    /// <summary>Each stage starts at its top: otherwise the next stage opens at the scroll position of the long previous one.</summary>
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ExposureViewModel.Mode))
        {
            Dispatcher.Dispatch(() => Scroller.ScrollToAsync(0, 0, false).FireAndForget());
        }
    }
}
