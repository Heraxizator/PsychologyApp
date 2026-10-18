using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileInsights;

public partial class InsightsPage : ContentPage
{
    private readonly InsightsViewModel _viewModel;

    public InsightsPage(IInsightsViewModelFactory viewModelFactory)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.Create(this);
        BindingContext = _viewModel;
    }

    // A practice or a test done from here changes the findings: they are read again whenever the page shows.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadAsync().FireAndForget();
    }
}
