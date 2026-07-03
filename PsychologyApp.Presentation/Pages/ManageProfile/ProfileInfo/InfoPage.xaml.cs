using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileInfo;

public partial class InfoPage : ContentPage
{
    public InfoPage(IPageViewModelActivator pageViewModelActivator, IInfoViewModelFactory infoViewModelFactory)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, page => infoViewModelFactory.Create(page));
    }
}
