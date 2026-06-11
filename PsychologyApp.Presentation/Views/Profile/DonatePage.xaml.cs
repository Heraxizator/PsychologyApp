using PsychologyApp.Presentation.ViewModels.Profile;

namespace PsychologyApp.Presentation.Views.Profile;

public partial class DonatePage : ContentPage
{
    public DonatePage(DonateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
