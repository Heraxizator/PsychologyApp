using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileDonate;

public partial class DonatePage : ContentPage
{
    public DonatePage(IDonateViewModelFactory donateViewModelFactory)
    {
        InitializeComponent();
        BindingContext = donateViewModelFactory.Create(this);
    }
}
