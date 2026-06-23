using PsychologyApp.Presentation.Pages.ProfileUser;

namespace PsychologyApp.Presentation.Pages.ProfileDonate;

public partial class DonatePage : ContentPage
{
    public DonatePage(DonateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
