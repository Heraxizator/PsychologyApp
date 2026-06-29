using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.ProfileDonate;

public partial class DonatePage : ContentPage
{
    public DonatePage(IDonateViewModelFactory donateViewModelFactory)
    {
        InitializeComponent();
        BindingContext = donateViewModelFactory.Create(this);
    }
}
