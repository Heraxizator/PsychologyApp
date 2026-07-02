using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(IPageViewModelActivator pageViewModelActivator, ISettingsViewModelFactory settingsViewModelFactory)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, page => settingsViewModelFactory.Create(page));
    }
}
