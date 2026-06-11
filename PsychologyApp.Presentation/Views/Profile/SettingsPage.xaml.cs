using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Profile;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(IPageViewModelActivator pageViewModelActivator, ISettingsViewModelFactory settingsViewModelFactory)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, nav => settingsViewModelFactory.Create(nav));
    }
}
