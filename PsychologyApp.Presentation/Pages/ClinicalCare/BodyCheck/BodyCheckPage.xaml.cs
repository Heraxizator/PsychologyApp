using PsychologyApp.Presentation.Features.ClinicalCare;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.BodyCheck;

public partial class BodyCheckPage : ContentPage
{
    private readonly BodyCheckViewModel _viewModel;

    public BodyCheckPage(IBodyCheckViewModelFactory viewModelFactory)
    {
        InitializeComponent();
        _viewModel = viewModelFactory.Create(this);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadAsync().FireAndForget();
    }
}
