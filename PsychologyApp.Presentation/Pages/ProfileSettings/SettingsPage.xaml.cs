using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.ProfileSettings;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(IPageViewModelActivator pageViewModelActivator, ISettingsViewModelFactory settingsViewModelFactory)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, nav => settingsViewModelFactory.Create(nav));
    }
}
