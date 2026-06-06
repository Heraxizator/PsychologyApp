namespace PsychologyApp.Presentation.Modules.Profile;

public partial class DonatePage : ContentPage
{
    public DonatePage(DonateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
