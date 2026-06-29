using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.ProfileInfo;

public partial class InfoPage : ContentPage
{
    public InfoPage(IPageViewModelActivator pageViewModelActivator, IInfoViewModelFactory infoViewModelFactory)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, page => infoViewModelFactory.Create(page));
    }
}
