using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Technique.Main;

namespace PsychologyApp.Presentation.Views;

public partial class TechniquesPage : ContentPage
{
    private TechniquesViewModel? _viewModel;

    public TechniquesPage(
        IPageViewModelActivator pageViewModelActivator,
        ITechniquesViewModelFactory techniquesViewModelFactory)
    {
        InitializeComponent();
        _viewModel = this.ActivateViewModel(pageViewModelActivator, nav => techniquesViewModelFactory.Create(nav));
    }

    protected override void OnDisappearing()
    {
        _viewModel?.Unsubscribe();
        base.OnDisappearing();
    }
}
