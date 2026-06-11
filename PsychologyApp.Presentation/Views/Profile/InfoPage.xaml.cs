using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Profile;

public partial class InfoPage : ContentPage
{
    public InfoPage(IPageViewModelActivator pageViewModelActivator, IInfoViewModelFactory infoViewModelFactory)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, nav => infoViewModelFactory.Create(nav));
    }
}
