using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Profile;

public partial class OptionsPage : ContentPage
{
    public OptionsPage(IPageViewModelActivator pageViewModelActivator, IOptionsViewModelFactory optionsViewModelFactory)
    {
        InitializeComponent();
        BindingContext = this.ActivateViewModel(pageViewModelActivator, nav => optionsViewModelFactory.Create(nav));
    }
}
